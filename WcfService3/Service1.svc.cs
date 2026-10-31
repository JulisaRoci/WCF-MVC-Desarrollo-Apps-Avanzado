using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfService3.Data;
using WcfService3.Models;

namespace WcfService3
{
       public class Service1 : IService1
    {
        public List<Producto> ListarProductos()
        {
            return DataProducto.ObtenerProductos();
        }

        public Producto ObtenerProducto(int id)
        {
            List<Producto> productos = DataProducto.ObtenerProductos();
            return productos.Find(p => p.Id == id); // Retorna el producto o null si no lo halla
        }


    }
}
