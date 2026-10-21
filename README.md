# Librerías Escuela

Sistema web para gestionar pedidos de útiles y libros escolares: listas por alumno, artículos con precios e imágenes, usuarios y reportes en PDF. Incluye un servicio de Windows que sincroniza los artículos desde Tango Gestión.

## Proyectos

| Proyecto | Tecnología | Descripción |
|---|---|---|
| [`Librerias`](Librerias) | ASP.NET Core (.NET 10), Razor Pages + Web API | Aplicación web. Corre en AWS Lambda detrás de API Gateway. |
| [`SincronizadorTangoLibrerias`](SincronizadorTangoLibrerias) | .NET Framework 4.8, servicio de Windows | Lee los artículos de Tango (SQL Server) y los envía a la API. |

## Librerias (aplicación web)

### Funcionalidades
- **Login** con cookie de autenticación y recupero de contraseña (el envío del mail por SES está deshabilitado en `Login.cshtml.cs`).
- **Listas / pedidos**: carga de pedidos por alumno, resumen y detalle.
- **Artículos**: listado y edición, con imágenes guardadas en S3.
- **Usuarios**: alta, edición, baja y blanqueo de clave.
- **Reportes**: generación de PDF a partir de un reporte Crystal (`reporte/Reporte.rpt`).

### Stack
- ASP.NET Core 10 (Razor Pages + controllers de API)
- Entity Framework Core con MySQL (`MySql.EntityFrameworkCore`)
- AWS: Lambda (`Amazon.Lambda.AspNetCoreServer`), API Gateway, S3 (imágenes y claves de Data Protection)

### Estructura
```
Librerias/
├── API/          Controllers de la Web API (requieren login, salvo Articulos/multiple)
├── Clases/       Funciones auxiliares y paginación
├── Models/       Entidades y LibreriasContext (EF Core)
├── Pages/        Razor Pages (Login, Listas, Articulos, Usuarios, ...)
├── Templates/    Plantillas de scaffolding
├── reporte/      Reporte Crystal
├── wwwroot/      Archivos estáticos (css, js, imágenes, plantillas de mail)
├── LambdaEntryPoint.cs   Entrada en AWS Lambda
├── LocalEntryPoint.cs    Entrada para correr en local
└── serverless.template   Plantilla de CloudFormation
```

### Configuración
La connection string **no está en el repositorio**. En `appsettings.json` queda vacía:

```json
"ConnectionStrings": {
  "db": ""
}
```

Se configura de alguna de estas formas:
- **En local**, con User Secrets:
  ```bash
  dotnet user-secrets set "ConnectionStrings:db" "Server=...;Database=Librerias;Uid=...;Pwd=..." --project Librerias
  ```
- **En AWS Lambda**, con la variable de entorno `ConnectionStrings__db`.

Para acceder a S3 se usan las credenciales por defecto de AWS (perfil local o el rol de la Lambda). No hay claves en el código.

### Correr en local
```bash
dotnet run --project Librerias
```

### Deploy a AWS
Con [Amazon.Lambda.Tools](https://github.com/aws/aws-extensions-for-dotnet-cli):
```bash
dotnet lambda deploy-serverless --project-location Librerias
```
Los valores por defecto (región, bucket, stack) están en `Librerias/aws-lambda-tools-defaults.json`.

## SincronizadorTangoLibrerias (servicio de Windows)

Servicio que se instala en el servidor donde corre Tango. Lee de SQL Server los artículos que cambiaron (detecta los cambios con un hash) y los envía en lotes al endpoint `POST /api/Articulos/multiple`.

- Se compila con Visual Studio (`SincronizadorTangoLibrerias.sln`).
- Se instala como servicio con `installutil` (`ProjectInstaller`).
- La configuración está en `App.config`.
