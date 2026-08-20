# Despliegue en Azure — FulbitoBravo

Guía para publicar el proyecto en **Azure App Service** con **Azure SQL
Database**. Se puede hacer desde Visual Studio, desde Visual Studio Code o
por CLI; aquí se detalla la vía CLI + Portal porque es la más reproducible.

## 1. Crear los recursos en Azure

```bash
# Variables (ajusta los nombres)
RG=rg-fulbitobravo
LOCATION=eastus
SQL_SERVER=sql-fulbitobravo
SQL_DB=DBGrassSintetico
SQL_ADMIN=fulbitoadmin
SQL_PASSWORD="CambiaEsto123!"
APP_PLAN=plan-fulbitobravo
APP_NAME=fulbitobravo-app   # debe ser único a nivel global

az login

az group create --name $RG --location $LOCATION

# --- Azure SQL ---
az sql server create --name $SQL_SERVER --resource-group $RG \
  --location $LOCATION --admin-user $SQL_ADMIN --admin-password $SQL_PASSWORD

# Permitir que los servicios de Azure (App Service) se conecten
az sql server firewall-rule create --resource-group $RG --server $SQL_SERVER \
  --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0

az sql db create --resource-group $RG --server $SQL_SERVER --name $SQL_DB \
  --service-objective Basic

# --- App Service (Linux, .NET 9) ---
az appservice plan create --name $APP_PLAN --resource-group $RG \
  --sku B1 --is-linux

az webapp create --resource-group $RG --plan $APP_PLAN --name $APP_NAME \
  --runtime "DOTNETCORE:9.0"
```

## 2. Ejecutar `BASE.sql` contra la base de datos de Azure

Desde **Azure Data Studio**, **SSMS** o `sqlcmd`, conéctate a
`sql-fulbitobravo.database.windows.net` con el usuario/clave creados
arriba y ejecuta `BASE.sql`.

> Nota: `BASE.sql` incluye un `USE master` inicial y un
> `CREATE DATABASE DBGrassSintetico` — en Azure SQL cada base de datos es
> un recurso independiente, así que si ya creaste la BD con `az sql db
> create`, **quita el bloque `CREATE DATABASE` / `DROP DATABASE`** del
> script y ejecuta solo desde `USE DBGrassSintetico;` en adelante,
> conectado directamente a esa base.

## 3. Configurar la cadena de conexión en App Service

No subas contraseñas reales a `appsettings.json`. Configúralas como
**Application Setting** en Azure (se inyectan como variables de entorno):

```bash
az webapp config connection-string set --resource-group $RG --name $APP_NAME \
  --connection-string-type SQLAzure \
  --settings CadenaSQL="Server=tcp:${SQL_SERVER}.database.windows.net,1433;Initial Catalog=${SQL_DB};User ID=${SQL_ADMIN};Password=${SQL_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

Esto sobreescribe automáticamente `ConnectionStrings:CadenaSQL` de
`appsettings.json` sin tocar el código fuente.

## 4. Publicar la aplicación

Desde la carpeta del proyecto:

```bash
dotnet publish -c Release -o ./publish

cd publish
zip -r ../publish.zip .
cd ..

az webapp deploy --resource-group $RG --name $APP_NAME \
  --src-path publish.zip --type zip
```

(Alternativa: botón derecho → "Publish" en Visual Studio, eligiendo el
App Service creado arriba.)

## 5. Verificar

- Abre `https://$APP_NAME.azurewebsites.net`
- Inicia sesión con `admin` / `Admin123!` (se crea solo la primera vez
  que la app arranca y encuentra la tabla `Usuario` vacía de Admins)
  y **cámbiala de inmediato**.
- Crea una cuenta de cliente desde "Crear cuenta" para probar el flujo
  completo de reserva.

## Notas y checklist final

- [ ] Cambiar la contraseña del admin por defecto (`Admin123!`).
- [ ] Revisar el firewall de Azure SQL (por defecto solo permite servicios
      de Azure; añade tu IP si necesitas conectarte desde tu equipo).
- [ ] Activar "HTTPS Only" en el App Service (Configuración → General).
- [ ] Si ves un bucle de redirección HTTPS, quita `app.UseHttpsRedirection()`
      de `Program.cs`: App Service ya termina TLS en el borde.
- [ ] Considera subir el `service-objective` de la base de datos
      (`Basic` → `S0` o superior) si esperas tráfico real.
- [ ] Activa backups automáticos de Azure SQL (vienen habilitados por
      defecto, pero revisa la retención).
- [ ] Rota manualmente `SQL_PASSWORD` después de la configuración inicial
      y actualiza el Connection String en App Service.
