using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FulbitoBravo.Data;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers;

[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
public class PagoController : Controller
{
    private readonly PagoRepositorio _pagoRepo;
    private readonly ReservaRepositorio _reservaRepo;

    public PagoController(PagoRepositorio pagoRepo, ReservaRepositorio reservaRepo)
    {
        _pagoRepo = pagoRepo;
        _reservaRepo = reservaRepo;
    }

    public IActionResult Index()
    {
        var pagos = _pagoRepo.Listar();
        ViewBag.TotalRecaudado = _pagoRepo.ObtenerTotalRecaudado();
        return View(pagos);
    }

    [HttpGet]
    public IActionResult Registrar()
    {
        ViewBag.ListaReservas = new SelectList(_reservaRepo.ListarPendientesDePago(), "IdReserva", "IdReserva");
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(PagoViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ListaReservas = new SelectList(_reservaRepo.ListarPendientesDePago(), "IdReserva", "IdReserva", modelo.IdReserva);
            return View(modelo);
        }

        _pagoRepo.Insertar(modelo);
        TempData["Mensaje"] = "Pago registrado exitosamente.";
        return RedirectToAction("Index");
    }
}