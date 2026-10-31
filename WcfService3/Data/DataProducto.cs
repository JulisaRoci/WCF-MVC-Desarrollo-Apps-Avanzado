using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using WcfService3.Models;

namespace WcfService3.Data
{
    public class DataProducto
    {
        public static List<Producto> ObtenerProductos()
        {
            return new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Cheesecake de Frutos Rojos", Precio = 18 },
                new Producto { Id = 2, Nombre = "Torta Tiramisú Artesanal", Precio = 22 },
                new Producto { Id = 3, Nombre = "Croissant relleno de Manjarblanco", Precio = 9 },
                new Producto { Id = 4, Nombre = "Pie de Limón Gourmet", Precio = 15 },
                new Producto { Id = 5, Nombre = "Brownie con Helado de Vainilla", Precio = 12 }
            };
        }

    }
}