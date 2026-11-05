using System;
using System.Collections.Generic;

namespace TestCliente.Models
{
    public class DetalleCertificadoItem
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    /// <summary>
    /// Modelo de datos para el Certificado Oficial de Devolución emitido por la plataforma
    /// </summary>
    public class CertificadoDevolucionViewModel
    {
        public string NumeroCertificado { get; set; }      // Ej: "CERT-DEV-2026-00042"
        public int IdDevolucion { get; set; }
        public int IdVenta { get; set; }
        public DateTime FechaEmision { get; set; }
        public string TipoDevolucion { get; set; }         // "PARCIAL" o "COMPLETA"
        public string MotivoDevolucion { get; set; }
        public decimal MontoTotalDevuelto { get; set; }
        public string UsuarioAutorizador { get; set; }
        public string ProtocoloSeguridad { get; set; }     // "WCF HTTPS (SSL/TLS 1.2)"
        public string HashFirmaDigital { get; set; }       // Código hash criptográfico
        public List<DetalleCertificadoItem> ItemsDevueltos { get; set; } = new List<DetalleCertificadoItem>();
    }
}
