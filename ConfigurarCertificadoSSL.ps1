<#
.SYNOPSIS
    Script para verificar o registrar el Certificado SSL Autofirmado para localhost (REQ 3 - DESAVA PA3).
    ISIL - Desarrollo de Aplicaciones Avanzado
.DESCRIPTION
    1. Verifica si existe un certificado SSL para 'localhost' en Cert:\LocalMachine\My.
    2. Comprueba si está registrado en Entidades de Certificación Raíz de Confianza (Root).
    3. Si falta algún paso, lo crea y lo añade (ejecutar como Administrador si es necesario).
#>

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  CONFIGURACION DEL CERTIFICADO SSL AUTOFIRMADO (REQ 3)  " -ForegroundColor Yellow
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Verificar si ya existe un certificado SSL para localhost en LocalMachine o CurrentUser
$cert = Get-ChildItem Cert:\LocalMachine\My | Where-Object { $_.Subject -eq "CN=localhost" } | Select-Object -First 1

if (-not $cert) {
    $cert = Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Subject -eq "CN=localhost" } | Select-Object -First 1
}

if ($cert) {
    Write-Host "[OK] Se encontro certificado SSL para 'localhost'." -ForegroundColor Green
    Write-Host "     Huella Digital (Thumbprint): $($cert.Thumbprint)" -ForegroundColor White
    Write-Host "     Valido hasta: $($cert.NotAfter)" -ForegroundColor White
} else {
    Write-Host "Creando nuevo Certificado SSL Autofirmado para 'localhost'..." -ForegroundColor Cyan
    $cert = New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "Cert:\LocalMachine\My" -FriendlyName "Certificado WCF HTTPS - DESAVA PA3" -NotAfter (Get-Date).AddYears(5) -KeyExportPolicy Exportable -KeySpec KeyExchange
    Write-Host "[OK] Certificado creado exitosamente." -ForegroundColor Green
    Write-Host "     Huella Digital (Thumbprint): $($cert.Thumbprint)" -ForegroundColor White
}

# 2. Verificar si está en Root de LocalMachine o CurrentUser
$enRootMachine = Get-ChildItem Cert:\LocalMachine\Root | Where-Object { $_.Thumbprint -eq $cert.Thumbprint }
$enRootUser    = Get-ChildItem Cert:\CurrentUser\Root | Where-Object { $_.Thumbprint -eq $cert.Thumbprint }

if ($enRootMachine -or $enRootUser) {
    Write-Host "[OK] El certificado YA esta registrado en 'Entidades de Certificacion Raiz de Confianza'." -ForegroundColor Green
    Write-Host "     Estado: 100% VALIDO y de CONFIANZA para Windows, IIS y Navegadores." -ForegroundColor Green
} else {
    Write-Host "Registrando en Entidades de Certificacion Raiz de Confianza..." -ForegroundColor Yellow
    try {
        $rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store "Root", "CurrentUser"
        $rootStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
        $rootStore.Add($cert)
        $rootStore.Close()
        Write-Host "[OK] Registrado exitosamente en CurrentUser\Root." -ForegroundColor Green
    } catch {
        Write-Warning "Para registrarlo a nivel de maquina ejecute PowerShell como Administrador."
    }
}

Write-Host "----------------------------------------------------------" -ForegroundColor DarkGray
Write-Host "Datos del Certificado para el Informe Escrito (REQ 3 / REQ 6):" -ForegroundColor Yellow
Write-Host "Emitido para (Subject) : $($cert.Subject)" -ForegroundColor White
Write-Host "Emitido por  (Issuer)  : $($cert.Issuer)" -ForegroundColor White
Write-Host "Huella digital (Hash)  : $($cert.Thumbprint)" -ForegroundColor White
Write-Host "Valido desde           : $($cert.NotBefore)" -ForegroundColor White
Write-Host "Valido hasta           : $($cert.NotAfter)" -ForegroundColor White
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "Configuracion completada con exito." -ForegroundColor Green
