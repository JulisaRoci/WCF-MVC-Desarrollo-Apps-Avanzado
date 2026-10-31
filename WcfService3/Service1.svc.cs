using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}