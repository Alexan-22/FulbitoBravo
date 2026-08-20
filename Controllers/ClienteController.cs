using Microsoft.AspNetCore.Mvc;
using FulbitoBravo.Data;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers;

[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
public class ClienteController : Controller
{
    private readonly ClienteRepositorio _repo;

    public ClienteController(ClienteRepositorio repo)
    {
        _repo = repo;
    }

    public IActionResult Index(string? buscar, int pagina = 1)
    {
        int tamanoPagina = 3; // para probar
        int totalRegistros;

        var clientes = _repo.ListarPaginado(buscar, pagina, tamanoPagina, out totalRegistros);

        // Datos que necesita la vista para la paginación
        ViewBag.Buscar = buscar;
        ViewBag.PaginaActual = pagina;
        ViewBag.TotalRegistros = totalRegistros;
        ViewBag.TamanoPagina = tamanoPagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanoPagina);

        return View(clientes);
    }

    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(ClienteViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        _repo.Insertar(modelo);
        TempData["Mensaje"] = "Cliente registrado exitosamente.";
        return RedirectToAction("Index");
    }
}