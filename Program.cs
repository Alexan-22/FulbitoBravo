using Microsoft.AspNetCore.Authentication.Cookies;
using FulbitoBravo.Data;
using FulbitoBravo.Models;
using FulbitoBravo.Seguridad;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar la cadena de conexión y todos los repositorios
builder.Services.AddSingleton<ConexionBD>();
builder.Services.AddScoped<ClienteRepositorio>();
builder.Services.AddScoped<CanchaRepositorio>();
builder.Services.AddScoped<HorarioRepositorio>();
builder.Services.AddScoped<ReservaRepositorio>();
builder.Services.AddScoped<PagoRepositorio>();
builder.Services.AddScoped<UsuarioRepositorio>();

// Autenticación por cookies (Admin / Cliente)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "FulbitoBravo.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ==========================================
// SEED: crea el usuario Admin por defecto si aún no existe ninguno.
// Usuario: admin   Contraseña: Admin123!
// El hash se genera con el mismo PasswordHasher que usa AuthController,
// por lo que funciona igual en local, en Docker o en Azure App Service.
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var usuarioRepo = scope.ServiceProvider.GetRequiredService<UsuarioRepositorio>();

    try
    {
        if (!usuarioRepo.ExisteAdmin())
        {
            usuarioRepo.Crear(new UsuarioViewModel
            {
                Username = "admin",
                PasswordHash = PasswordHasher.Hash("Admin123!"),
                Rol = "Admin",
                IdCliente = null,
                Activo = true
            });
        }
    }
    catch (Exception ex)
    {
        // No se pudo conectar a la BD al arrancar (p. ej. primer despliegue
        // antes de ejecutar BASE.sql). Se registra el error y la app sigue
        // levantando; el seed se reintentará en el próximo arranque.
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "No se pudo verificar/crear el usuario Admin por defecto al iniciar.");
    }
}

app.Run();
