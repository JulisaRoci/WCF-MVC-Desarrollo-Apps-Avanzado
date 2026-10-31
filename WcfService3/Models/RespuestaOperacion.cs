using System.Runtime.Serialization;

namespace WcfService3.Models
{
    [DataContract]
    public class RespuestaOperacion
    {
        [DataMember]
        public bool Exito { get; set; }

        [DataMember]
        public string Mensaje { get; set; }
    }

    [DataContract]
    public class ProductoDTO
    {
        [DataMember] public int Id { get; set; }
        [DataMember] public string Nombre { get; set; }
        [DataMember] public string Descripcion { get; set; }
        [DataMember] public decimal Precio { get; set; }
        [DataMember] public int Stock { get; set; }
        [DataMember] public int CategoriaId { get; set; }
        [DataMember] public string CategoriaNombre { get; set; }
        [DataMember] public string UrlImagen { get; set; }
    }
}