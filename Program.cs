using FulbitoBravo.Data;

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();