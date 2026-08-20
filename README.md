# FulbitoBravo

Sistema web (ASP.NET Core MVC + SQL Server) para la gestión de un complejo
de canchas de fulbito (grass sintético): clientes, canchas, horarios,
reservas y pagos, con **login diferenciado para Administrador y Cliente**.

## Tecnología

- ASP.NET Core 9 MVC (Razor Views)
- ADO.NET puro (`Microsoft.Data.SqlClient`) — sin Entity Framework
- SQL Server (local `SQLEXPRESS` en desarrollo, Azure SQL en producción)
- Autenticación por cookies (`Microsoft.AspNetCore.Authentication.Cookies`)
- Bootstrap 5 + Bootstrap Icons + tema visual propio (`wwwroot/css/site.css`)

## Roles

| Rol      | Acceso |
|----------|--------|
| **Admin**   | CRUD completo de Clientes, Canchas, Horarios, Reservas, Pagos, Reporte de reservas y toda la API REST (`/api/...`). |
| **Cliente** | Se autorregistra desde `/Auth/Registro`. Puede reservar canchas para sí mismo y ver "Mis Reservas". |

El primer usuario **Admin** se crea automáticamente al arrancar la
aplicación si aún no existe ninguno:

```
Usuario:    admin
Contraseña: Admin123!
```



## Puesta en marcha en local

1. Ejecuta `BASE.sql` en tu instancia de SQL Server (crea la base de
   datos `DBGrassSintetico`, tablas, datos de prueba y procedimientos
   almacenados).
2. Ajusta la cadena de conexión en `appsettings.Development.json`.
3. `dotnet restore && dotnet run`
4. Abre `https://localhost:xxxx`, entra con `admin` / `Admin123!`, o
   crea una cuenta de cliente desde "Crear cuenta".

## Despliegue en Azure

Ver [`DEPLOY.md`](./DEPLOY.md) para la guía paso a paso (Azure App
Service + Azure SQL Database).
