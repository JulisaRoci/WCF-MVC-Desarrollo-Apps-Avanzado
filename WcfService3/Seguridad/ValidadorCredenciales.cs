using System;
using System.IdentityModel.Selectors;
using System.ServiceModel;

namespace WcfService3.Seguridad
{
    /// <summary>
    /// REQ 4: Validador de credenciales autorizadas para consumir las operaciones del servicio WCF.
    /// Compatible con autenticación básica y credenciales de usuario.
    /// </summary>
    public class ValidadorCredenciales : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                throw new FaultException("Acceso Denegado: Debe ingresar usuario y contraseña.");
            }

            // Credenciales autorizadas para consumir el servicio WCF
            bool esValido = (userName == "admin" && password == "Isil2026*") ||
                            (userName == "supervisor" && password == "Devolucion123*") ||
                            (userName == "usuario" && password == "Desava2026*");

            if (!esValido)
            {
                throw new FaultException($"Acceso Denegado: El usuario '{userName}' o la contraseña ingresada son incorrectos.");
            }
        }
    }
}
