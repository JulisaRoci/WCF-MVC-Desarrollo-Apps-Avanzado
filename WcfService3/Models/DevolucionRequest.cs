using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WcfService3.Models
{
    [DataContract]
    public class DetalleDevolucionRequest
    {
        [DataMember] public int IdProducto { get; set; }
        [DataMember] public int Cantidad { get; set; }
    }

    [DataContract]
    public class DevolucionRequest
    {
        [DataMember] public int IdVenta { get; set; }
        [DataMember] public string MotivoDevolucion { get; set; }
        [DataMember] public List<DetalleDevolucionRequest> Detalles { get; set; }
    }
}