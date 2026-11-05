# GUÍA OFICIAL: IMPLEMENTACIÓN DEL CERTIFICADO Y COMUNICACIÓN SEGURA (REQ 3, REQ 4, REQ 5 Y REQ 6)
**Curso:** 4619 - Desarrollo de Aplicaciones Avanzado  
**Evaluación:** Avance de Proyecto 3 (PA3)  
**Institución:** ISIL  

---

## 1. Resumen de la Asignación y Requerimientos Cumplidos

En el documento oficial de evaluación (**DESAVA - AVANCE DE PROYECTO 3**), la funcionalidad de **"El Certificado"** corresponde formalmente a:
- **REQ 3. Configuración de comunicación segura mediante HTTPS (Certificado SSL Autofirmado):**
  1. Certificado SSL autofirmado emitido para `localhost` e instalado en el almacén de confianza.
  2. Enlace HTTPS asociado al sitio/puerto en IIS e IIS Express.
  3. Configuración de endpoints seguros en `Web.config` mediante `basicHttpsBinding` con `security mode="Transport"`.
  4. Verificación de publicación accesible por URL segura (`https://localhost:44350/Service1.svc`).
- **REQ 4. Implementación de autenticación básica:**
  1. Mecanismo de autenticación básica sobre transporte SSL/HTTPS.
  2. Credenciales autorizadas para consumir las operaciones del servicio.
- **REQ 5. Pruebas y Evidencias:**
  1. Pruebas con `WcfTestClient` de la operación `RegistrarDevolucionVenta` (2 casos exitosos y validaciones de error).
  2. Acceso al servicio publicado con credenciales y HTTPS.
  3. Extracción del WSDL en formato XML.
- **Módulo Opcional de Diseño (Frontend MVC):**
  1. Vistas Razor (`Certificado.cshtml` y `Registrar.cshtml`) y Controller para generar el comprobante visual y voucher oficial de la devolución.

---

## 2. Archivos Desarrollados e Integrados en el Proyecto

| Componente | Archivo | Función |
| :--- | :--- | :--- |
| **Certificado SSL** | `ConfigurarCertificadoSSL.ps1` | Script automatizado para generar y validar el certificado `localhost` en `Root`. |
| **Configuración WCF** | `WcfService3/Web.config` | Endpoints `basicHttpsBinding`, `mexHttpsBinding`, seguridad `Transport` y `Basic`. |
| **Proyecto WCF** | `WcfService3/WcfService3.csproj` | Habilitación de puerto SSL IIS Express (`44350`) y referencias seguras. |
| **Seguridad WCF** | `WcfService3/Seguridad/ValidadorCredenciales.cs` | Validador de credenciales autorizadas en C# (`UserNamePasswordValidator`). |
| **Modelo MVC** | `ClienteMVC_ModuloCertificado/Models/CertificadoDevolucionViewModel.cs` | Modelo para voucher/certificado de devolución. |
| **Controlador MVC** | `ClienteMVC_ModuloCertificado/Controllers/DevolucionController.cs` | Acción que emite el certificado tras registrar la devolución. |
| **Vista MVC** | `ClienteMVC_ModuloCertificado/Views/Devolucion/Certificado.cshtml` | Diseño formal con sello, folio, desglose y CSS para impresión en PDF. |

---

## 3. Instrucciones Paso a Paso para Ejecución y Capturas

### Paso 1: Certificado SSL Autofirmado (REQ 3)
1. Abrir PowerShell en la raíz del proyecto.
2. Ejecutar el script:
   ```powershell
   powershell -ExecutionPolicy Bypass -File .\ConfigurarCertificadoSSL.ps1
   ```
3. Se verificará la huella digital (`26F13E1FE1FB0AB135738759D01713CCAF21DD11`) y el estado de confianza en Windows.
4. **Captura para el informe:** Tomar print de la salida de la consola mostrando el certificado generado y válido.

### Paso 2: Ejecutar el Servicio WCF en Visual Studio (HTTPS)
1. Abrir la solución `WcfService3.slnx` en **Visual Studio 2022 / 2026**.
2. Hacer clic derecho sobre `WcfService3` > **Establecer como proyecto de inicio**.
3. Hacer clic derecho sobre `Service1.svc` > **Ver en el explorador**.
4. Se abrirá el navegador en la URL segura:
   ```
   https://localhost:44350/Service1.svc
   ```
5. Si se configuró en IIS con autenticación básica, el navegador solicitará usuario y contraseña:
   - **Usuario:** `admin`
   - **Contraseña:** `Isil2026*`
6. Se desplegará la pantalla de confirmación: *"Service1 Service: Se ha creado un servicio..."*.
7. **Captura para el informe:** Print de la URL con candado verde/seguro `https://...` y el navegador autenticado.

### Paso 3: Guardar el Archivo WSDL XML (REQ 5)
1. En el navegador, ingresar a:
   ```
   https://localhost:44350/Service1.svc?wsdl
   ```
2. Presionar `Ctrl + S` y guardar el archivo como `Service1.wsdl.xml` para adjuntarlo en la entrega grupal.

### Paso 4: Pruebas en WcfTestClient (REQ 5)
1. Ejecutar el cliente de pruebas de Visual Studio ubicado en:
   `C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\WcfTestClient.exe`
2. Menú **Archivo** > **Agregar servicio...**
3. Ingresar la dirección del servicio:
   ```
   https://localhost:44350/Service1.svc
   ```
4. Probar la operación `RegistrarDevolucionVenta`:
   - **Caso 1 (Éxito - Devolución Parcial):**
     - `IdVenta`: 1
     - `MotivoDevolucion`: "Cambio por talla/defecto"
     - `Detalles`: `IdProducto` = 1, `Cantidad` = 1
     - *Resultado esperado:* `Exito = true`, Mensaje con ID de devolución y Tipo: "PARCIAL".
   - **Caso 2 (Éxito - Devolución Completa):**
     - Devolver los productos restantes de la venta hasta completar el total vendido.
     - *Resultado esperado:* `Exito = true`, Tipo: "COMPLETA".
   - **Caso 3 (Error - Cantidad Excedida):**
     - `Cantidad` mayor al saldo pendiente de la venta.
     - *Resultado esperado:* `Exito = false`, Mensaje detallando el disponible máximo.
   - **Caso 4 (Error - Cantidad <= 0):**
     - `Cantidad` = 0 o negativo.
     - *Resultado esperado:* `Exito = false`, Mensaje de cantidad mayor a cero.
   - **Caso 5 (Error - Producto no pertenece a la venta):**
     - `IdProducto` inexistente en la venta original.
     - *Resultado esperado:* `Exito = false`, Mensaje de producto no pertenece.

---

## 4. Guía para el Informe de Desarrollo (REQ 6)

El informe debe estructurarse conforme a la página 5 del documento de evaluación:
1. **Resumen Ejecutivo:**
   Descripción de la ampliación de WCF para soportar transacciones de devolución, aseguramiento criptográfico HTTPS mediante certificados SSL y autenticación básica.
2. **Desarrollo:**
   - **4.1 Funcionalidades implementadas:**
     - Configuración de `Web.config` con `basicHttpsBinding` y transporte protegido por certificado SSL.
     - Implementación de la seguridad y validación de credenciales.
     - Explicación de la transacción ACID (`TransactionScope`) en `RegistrarDevolucionVenta`.
   - **4.2 Capturas de Pantalla:**
     - Print del script de creación y almacén de certificados (`certmgr.msc` o consola).
     - Print del enlace HTTPS en IIS / IIS Express.
     - Print de la estructura de `Web.config`.
     - Print del acceso por navegador a `https://localhost:44350/Service1.svc`.
   - **4.3 Pruebas:**
     - Prints de los 2 casos exitosos en WcfTestClient.
     - Prints de los escenarios de error de validación en WcfTestClient.
3. **Anexo:**
   - Archivo XML del WSDL descargado.
