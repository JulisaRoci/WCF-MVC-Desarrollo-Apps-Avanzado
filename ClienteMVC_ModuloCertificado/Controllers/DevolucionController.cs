using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using TestCliente.Models;

namespace TestCliente.Controllers
{
    public class DevolucionController : Controller
    {
        // GET: Devolucion/Registrar
        public ActionResult Registrar()
        {
            return View();
        }

        // GET: Devolucion/Certificado/{id}
        // Acción encargada de generar y desplegar el Certificado / Comprobante de Devolución
        public ActionResult Certificado(int idDevolucion, int idVenta, string motivo, string tipo, decimal total, string productosJson = null)
        {
            // Generar Hash criptográfico representativo del certificado para trazabilidad
            string rawData = $"{idDevolucion}-{idVenta}-{DateTime.Now:yyyyMMddHHmmss}-{total}";
            string hashFirma;
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                hashFirma = BitConverter.ToString(bytes).Replace("-", "").Substring(0, 24);
            }

            var model = new CertificadoDevolucionViewModel
            {
                NumeroCertificado = $"CERT-DEV-{DateTime.Now.Year}-{idDevolucion.ToString().PadLeft(5, '0')}",
                IdDevolucion = idDevolucion,
                IdVenta = idVenta,
                FechaEmision = DateTime.Now,
                TipoDevolucion = string.IsNullOrEmpty(tipo) ? "PARCIAL" : tipo.ToUpper(),
                MotivoDevolucion = motivo ?? "Devolución regular de producto por garantía/cambio",
                MontoTotalDevuelto = total,
                UsuarioAutorizador = "admin (Credenciales Básicas Autorizadas)",
                ProtocoloSeguridad = "WCF HTTPS SSL / TLS (Endpoint Seguro)",
                HashFirmaDigital = hashFirma,
                ItemsDevueltos = new List<DetalleCertificadoItem>
                {
                    // Ejemplo de items vinculados para la representación visual del comprobante
                    new DetalleCertificadoItem { IdProducto = 1, NombreProducto = "Producto Devuelto Registrado", Cantidad = 1, PrecioUnitario = total }
                }
            };

            return View(model);
        }
    }
}
