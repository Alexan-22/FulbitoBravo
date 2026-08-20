using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FulbitoBravo.Data;
using FulbitoBravo.Models;
using FulbitoBravo.Models.Auth;
using FulbitoBravo.Seguridad;

namespace FulbitoBravo.Controllers;

public class AuthController : Controller
{
    private readonly UsuarioRepositorio _usuarioRepo;
    private readonly ClienteRepositorio _clienteRepo;

    public AuthController(UsuarioRepositorio usuarioRepo, ClienteRepositorio clienteRepo)
    {
        _usuarioRepo = usuarioRepo;
        _clienteRepo = clienteRepo;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var usuario = _usuarioRepo.ObtenerPorUsername(modelo.Username.Trim());

        if (usuario == null || !usuario.Activo || !PasswordHasher.Verify(modelo.Password, usuario.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View(modelo);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.Username),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        if (usuario.IdCliente.HasValue)
        {
            claims.Add(new Claim("IdCliente", usuario.IdCliente.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });

        if (!string.IsNullOrEmpty(modelo.ReturnUrl) && Url.IsLocalUrl(modelo.ReturnUrl))
            return Redirect(modelo.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Registro()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new RegistroViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Registro(RegistroViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        if (_clienteRepo.ObtenerPorDNI(modelo.DNI) != null)
        {
            ModelState.AddModelError(nameof(modelo.DNI), "Ya existe un cliente registrado con ese DNI.");
            return View(modelo);
        }

        if (_usuarioRepo.ExisteUsername(modelo.Username))
        {
            ModelState.AddModelError(nameof(modelo.Username), "Ese nombre de usuario ya está en uso.");
            return View(modelo);
        }

        // 1. Crear la ficha de Cliente
        var cliente = new ClienteViewModel
        {
            DNI = modelo.DNI,
            Nombre = modelo.Nombre,
            Apellido = modelo.Apellido,
            Telefono = modelo.Telefono,
            Correo = modelo.Correo
        };

        int idCliente = _clienteRepo.Insertar(cliente);

        // 2. Crear la cuenta de Usuario enlazada, con Rol = Cliente
        var usuario = new UsuarioViewModel
        {
            Username = modelo.Username,
            PasswordHash = PasswordHasher.Hash(modelo.Password),
            Rol = "Cliente",
            IdCliente = idCliente,
            Activo = true
        };

        _usuarioRepo.Crear(usuario);

        TempData["Mensaje"] = "Cuenta creada exitosamente. Ya puedes iniciar sesión.";
        return RedirectToAction("Login");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
