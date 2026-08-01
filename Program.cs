using FulbitoBravo.Data;
using FulbitoBravo.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Singleton para la clase de conexión a BD
builder.Services.AddSingleton<ConexionBD>();

// 2. Inyección de Dependencias usando Interfaz
builder.Services.AddScoped<IReservaRepositorio, ReservaRepositorio>();

// 3. Demás repositorios del sistema
builder.Services.AddScoped<ClienteRepositorio>();
builder.Services.AddScoped<CanchaRepositorio>();
builder.Services.AddScoped<HorarioRepositorio>();
builder.Services.AddScoped<PagoRepositorio>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cliente}/{action=Index}/{id?}");

app.Run();