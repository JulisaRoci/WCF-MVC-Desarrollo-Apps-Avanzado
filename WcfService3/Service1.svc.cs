using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using WcfService3.Models;

namespace WcfService3
{
    public class Service1 : IService1
    {
        // 1. LISTAR PRODUCTOS
        public List<sp_ListarProductos_Result> ListarProductos()
        {
            using (var db = new TiendaDBEntities())
            {
                return db.sp_ListarProductos().ToList();
            }
        }

        // 2. OBTENER PRODUCTO POR ID
        public sp_ObtenerProductoPorId_Result ObtenerProductoPorId(int id)
        {
            using (var db = new TiendaDBEntities())
            {
                var producto = db.sp_ObtenerProductoPorId(id).FirstOrDefault();
                return producto; // Si no existe retorna null
            }
        }

        // 3. INSERTAR PRODUCTO CON VALIDACIONES
        public RespuestaOperacion InsertarProducto(ProductoDTO producto)
        {
            if (producto == null)
                return new RespuestaOperacion { Exito = false, Mensaje = "Los datos del producto son nulos." };

            // Validaciones
            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return new RespuestaOperacion { Exito = false, Mensaje = "El nombre del producto es obligatorio." };

            if (producto.Precio <= 0)
                return new RespuestaOperacion { Exito = false, Mensaje = "El precio debe ser mayor a cero." };

            if (producto.Stock < 0)
                return new RespuestaOperacion { Exito = false, Mensaje = "El stock debe ser un valor válido (mayor o igual a cero)." };

            using (var db = new TiendaDBEntities())
            {
                // Validación: Categoría existente
                bool categoriaExiste = db.Categorias.Any(c => c.Id == producto.CategoriaId);
                if (!categoriaExiste)
                    return new RespuestaOperacion { Exito = false, Mensaje = $"La categoría con ID {producto.CategoriaId} no existe." };

                // Ejecución de SP
                db.sp_InsertarProducto(
                    producto.Nombre,
                    producto.Descripcion,
                    producto.Precio,
                    producto.Stock,
                    producto.CategoriaId,
                    producto.UrlImagen
                );

                return new RespuestaOperacion { Exito = true, Mensaje = "Producto registrado exitosamente mediante SP." };
            }
        }

        // 4. ACTUALIZAR PRODUCTO CON VALIDACIONES
        public RespuestaOperacion ActualizarProducto(ProductoDTO producto)
        {
            if (producto == null)
                return new RespuestaOperacion { Exito = false, Mensaje = "Los datos del producto son nulos." };

            if (producto.Id <= 0)
                return new RespuestaOperacion { Exito = false, Mensaje = "El ID del producto es inválido." };

            // Validaciones
            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return new RespuestaOperacion { Exito = false, Mensaje = "El nombre es obligatorio." };

            if (producto.Precio <= 0)
                return new RespuestaOperacion { Exito = false, Mensaje = "El precio debe ser mayor a cero." };

            if (producto.Stock < 0)
                return new RespuestaOperacion { Exito = false, Mensaje = "El stock no puede ser negativo." };

            using (var db = new TiendaDBEntities())
            {
                // Validación: Producto existente para UPDATE
                bool productoExiste = db.Productos.Any(p => p.Id == producto.Id);
                if (!productoExiste)
                    return new RespuestaOperacion { Exito = false, Mensaje = $"No existe un producto registrado con el ID {producto.Id}." };

                // Validación: Categoría existente
                bool categoriaExiste = db.Categorias.Any(c => c.Id == producto.CategoriaId);
                if (!categoriaExiste)
                    return new RespuestaOperacion { Exito = false, Mensaje = $"La categoría con ID {producto.CategoriaId} no existe." };

                // Ejecución de SP
                db.sp_ActualizarProducto(
                    producto.Id,
                    producto.Nombre,
                    producto.Descripcion,
                    producto.Precio,
                    producto.Stock,
                    producto.CategoriaId,
                    producto.UrlImagen
                );

                return new RespuestaOperacion { Exito = true, Mensaje = "Producto actualizado correctamente." };
            }
        }

        // 5. ELIMINAR PRODUCTO
        public RespuestaOperacion EliminarProducto(int id)
        {
            using (var db = new TiendaDBEntities())
            {
                // Validación: Producto existente
                bool productoExiste = db.Productos.Any(p => p.Id == id);
                if (!productoExiste)
                {
                    return new RespuestaOperacion
                    {
                        Exito = false,
                        Mensaje = $"No se pudo eliminar: El producto con ID {id} no fue encontrado en la base de datos."
                    };
                }

                // Ejecución de SP
                db.sp_EliminarProducto(id);

                return new RespuestaOperacion
                {
                    Exito = true,
                    Mensaje = $"El producto con ID {id} fue eliminado correctamente."
                };
            }
        }


        // 6. REGISTRAR DEVOLUCIÓN DE VENTA
        public RespuestaOperacion RegistrarDevolucionVenta(DevolucionRequest solicitud)
        {
            // 1. Validaciones Iniciales
            if (solicitud == null)
                return new RespuestaOperacion { Exito = false, Mensaje = "La solicitud de devolución es nula." };

            if (solicitud.IdVenta <= 0)
                return new RespuestaOperacion { Exito = false, Mensaje = "El ID de la venta es inválido." };

            if (string.IsNullOrWhiteSpace(solicitud.MotivoDevolucion))
                return new RespuestaOperacion { Exito = false, Mensaje = "El motivo de la devolución es obligatorio." };

            if (solicitud.Detalles == null || solicitud.Detalles.Count == 0)
                return new RespuestaOperacion { Exito = false, Mensaje = "Debe proporcionar al menos un detalle de devolución." };

            // 2. Control de Transacción con TransactionScope (REQ2)
            using (TransactionScope scope = new TransactionScope())
            {
                using (var db = new TiendaDBEntities())
                {
                    // A. Verificar que la venta exista
                    var venta = db.Ventas.FirstOrDefault(v => v.Id == solicitud.IdVenta);
                    if (venta == null)
                        return new RespuestaOperacion { Exito = false, Mensaje = $"No existe una venta registrada con el ID {solicitud.IdVenta}." };

                    // B. Obtener los detalles de la venta mediante la propiedad de navegación o el DbContext
                    var detallesVenta = db.DetalleVentas.Where(dv => dv.Venta.Id == solicitud.IdVenta).ToList();
                    decimal montoTotalDevuelto = 0;

                    // C. Obtener devoluciones previas registradas usando las colecciones exactas de tu diagrama
                    var devolucionesPrevias = db.Devolucions
                        .Where(d => d.Venta.Id == solicitud.IdVenta)
                        .SelectMany(d => d.DetalleDevolucions)
                        .ToList();

                    // D. Validaciones por cada producto a devolver
                    foreach (var item in solicitud.Detalles)
                    {
                        if (item.Cantidad <= 0)
                            return new RespuestaOperacion { Exito = false, Mensaje = $"La cantidad a devolver para el producto ID {item.IdProducto} debe ser mayor a cero." };

                        var detalleOriginal = detallesVenta.FirstOrDefault(dv => dv.Producto.Id == item.IdProducto);
                        if (detalleOriginal == null)
                            return new RespuestaOperacion { Exito = false, Mensaje = $"El producto ID {item.IdProducto} no pertenece a la venta {solicitud.IdVenta}." };

                        int yaDevuelto = devolucionesPrevias
                            .Where(dd => dd.Producto.Id == item.IdProducto)
                            .Sum(dd => (int?)dd.Cantidad) ?? 0;

                        int disponibleParaDevolver = detalleOriginal.Cantidad - yaDevuelto;

                        if (item.Cantidad > disponibleParaDevolver)
                        {
                            return new RespuestaOperacion
                            {
                                Exito = false,
                                Mensaje = $"No se puede devolver {item.Cantidad} unidad(es) del producto ID {item.IdProducto}. Disponible máximo: {disponibleParaDevolver} unidad(es)."
                            };
                        }

                        montoTotalDevuelto += item.Cantidad * detalleOriginal.Precio;
                    }

                    // E. Determinar si es Devolución PARCIAL o COMPLETA
                    int totalItemsVendidos = detallesVenta.Sum(dv => dv.Cantidad);
                    int totalItemsDevueltosPrevios = devolucionesPrevias.Sum(dd => dd.Cantidad);
                    int totalItemsSolicitados = solicitud.Detalles.Sum(d => d.Cantidad);

                    string tipoDevolucion = (totalItemsDevueltosPrevios + totalItemsSolicitados >= totalItemsVendidos) ? "COMPLETA" : "PARCIAL";

                    // F. Registrar Cabecera asociando la entidad Venta
                    var nuevaDevolucion = new Devolucion
                    {
                        Venta = venta,
                        Fecha = DateTime.Now,
                        MotivoDevolucion = solicitud.MotivoDevolucion,
                        TipoDevolucion = tipoDevolucion,
                        MontoTotalDevuelto = montoTotalDevuelto
                    };
                    db.Devolucions.Add(nuevaDevolucion);
                    db.SaveChanges(); // Guarda y genera el ID de la Devolución

                    // G. Registrar Detalle asociando las entidades Producto y Devolucion
                    foreach (var item in solicitud.Detalles)
                    {
                        var detalleOriginal = detallesVenta.First(dv => dv.Producto.Id == item.IdProducto);
                        var productoBD = db.Productos.FirstOrDefault(p => p.Id == item.IdProducto);

                        var nuevoDetalleDev = new DetalleDevolucion
                        {
                            Devolucion = nuevaDevolucion,
                            Producto = productoBD,
                            Cantidad = item.Cantidad,
                            PrecioVenta = detalleOriginal.Precio
                        };
                        db.DetalleDevolucions.Add(nuevoDetalleDev);

                        // Actualización de Stock en la entidad Producto
                        if (productoBD != null)
                        {
                            productoBD.Stock += item.Cantidad;
                        }
                    }

                    db.SaveChanges();

                    // Confirmar Transacción
                    scope.Complete();

                    return new RespuestaOperacion
                    {
                        Exito = true,
                        Mensaje = $"Devolución registrada exitosamente (ID Devolución: {nuevaDevolucion.Id}). Tipo: {tipoDevolucion}. Monto devuelto: S/ {montoTotalDevuelto}."
                    };
                }
            }

        }
    }
}