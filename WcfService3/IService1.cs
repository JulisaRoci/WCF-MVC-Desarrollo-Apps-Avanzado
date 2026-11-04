using System.Collections.Generic;
using System.ServiceModel;
using WcfService3.Models;

namespace WcfService3
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        List<sp_ListarProductos_Result> ListarProductos();

        [OperationContract]
        sp_ObtenerProductoPorId_Result ObtenerProductoPorId(int id);

        [OperationContract]
        RespuestaOperacion InsertarProducto(ProductoDTO producto);

        [OperationContract]
        RespuestaOperacion ActualizarProducto(ProductoDTO producto);

        [OperationContract]
        RespuestaOperacion EliminarProducto(int id);

        [OperationContract]
        RespuestaOperacion RegistrarDevolucionVenta(DevolucionRequest solicitud);

    }
}