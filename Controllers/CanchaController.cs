using Microsoft.AspNetCore.Mvc;
using FulbitoBravo.Data;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers;

[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
public class CanchaController : Controller
{
    private readonly CanchaRepositorio _repo;

    public CanchaController(CanchaRepositorio repo) => _repo = repo;

    public IActionResult Index()
    {
        var canchas = _repo.Listar();
        return View(canchas);
    }

    [HttpGet]
    public IActionResult Registrar() => View();

    [HttpPost]
    public IActionResult Registrar(CanchaViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);
        _repo.Insertar(modelo);
        TempData["Mensaje"] = $"Cancha '{modelo.Nombre}' registrada exitosamente.";
        return RedirectToAction("Index");
    }

    [HttpPost]
public IActionResult CambiarEstado(int id, bool estado)
    {
        var resultado = _repo.CambiarEstado(id, estado);

        if (resultado)
        {
            TempData["Mensaje"] = estado
                ? "Cancha activada correctamente."
                : "Cancha desactivada correctamente.";
        }
        else
        {
            TempData["Mensaje"] = "No se pudo cambiar el estado de la cancha.";
        }

        return RedirectToAction("Index");
    }
}