using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FulbitoBravo.Data;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers;

public class ReservaController : Controller
{
    private readonly ReservaRepositorio _reservaRepo;
    private readonly ClienteRepositorio _clienteRepo;
    private readonly CanchaRepositorio _canchaRepo;
    private readonly HorarioRepositorio _horarioRepo;

    public ReservaController(
        ReservaRepositorio reservaRepo, 
        ClienteRepositorio clienteRepo, 
        CanchaRepositorio canchaRepo, 
        HorarioRepositorio horarioRepo)
    {
        _reservaRepo = reservaRepo;
        _clienteRepo = clienteRepo;
        _canchaRepo = canchaRepo;
        _horarioRepo = horarioRepo;
    }

    public IActionResult Index(int pagina = 1)
    {
        int tamanoPagina = 3; // Para probar
        int totalRegistros;

        // Sin filtro, muchas las fechas mas recientes primero
        var reservas = _reservaRepo.ListarPaginado(null, null, pagina, tamanoPagina, out totalRegistros);

        ViewBag.PaginaActual = pagina;
        ViewBag.TotalRegistros = totalRegistros;
        ViewBag.TamanoPagina = tamanoPagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanoPagina);

        return View(reservas);
    }

    [HttpGet]
    public IActionResult Registrar()
    {
        ViewBag.ListaClientes = new SelectList(_clienteRepo.Listar(""), "IdCliente", "Nombre");
        ViewBag.ListaCanchas = new SelectList(_canchaRepo.Listar(), "IdCancha", "Nombre");
        ViewBag.ListaHorarios = new SelectList(_horarioRepo.Listar(), "IdHorario", "HoraInicio");
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(ReservaViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ListaClientes = new SelectList(_clienteRepo.Listar(""), "IdCliente", "Nombre", modelo.IdCliente);
            ViewBag.ListaCanchas = new SelectList(_canchaRepo.Listar(), "IdCancha", "Nombre", modelo.IdCancha);
            ViewBag.ListaHorarios = new SelectList(_horarioRepo.Listar(), "IdHorario", "HoraInicio", modelo.IdHorario);
            return View(modelo);
        }

        _reservaRepo.Insertar(modelo);
        TempData["Mensaje"] = "Reserva registrada exitosamente.";
        return RedirectToAction("Index");
    }
}