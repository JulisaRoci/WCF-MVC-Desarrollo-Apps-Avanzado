using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfService3.Models
{
    public class Producto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public decimal Precio { get; set; }
        [DataMember]
        public int Stock { get; set; }
        [DataMember]
        public int CategoriaId { get; set; }
        [DataMember]
        public string CategoriaNombre { get; set; }
        [DataMember]
        public string UrlImagen { get; set; }

    }
}