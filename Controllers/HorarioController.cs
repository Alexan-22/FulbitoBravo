using Microsoft.AspNetCore.Mvc;
using FulbitoBravo.Data;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers;

[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
public class HorarioController : Controller
{
    private readonly HorarioRepositorio _repo;

    public HorarioController(HorarioRepositorio repo)
    {
        _repo = repo;
    }

    public IActionResult Index()
    {
        var horarios = _repo.Listar();

        return View(horarios);
    }

    [HttpGet]
    public IActionResult Registrar()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(HorarioViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        _repo.Insertar(modelo);

        TempData["Mensaje"] = "Horario registrado exitosamente.";

        return RedirectToAction("Index");
    }
}