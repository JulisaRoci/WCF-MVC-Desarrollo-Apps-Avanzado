using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfService3.Models;

namespace WcfService3
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        List<Producto> ListarProductos();

        [OperationContract]
        Producto ObtenerProducto(int id);
    }

}
