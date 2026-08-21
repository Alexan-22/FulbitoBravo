using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FulbitoBravo.Data;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers;

[Authorize]
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

    private int? IdClienteActual
    {
        get
        {
            var valor = User.Claims.FirstOrDefault(c => c.Type == "IdCliente")?.Value;
            return int.TryParse(valor, out int id) ? id : null;
        }
    }

    // Listado completo: solo Admin.
    [Authorize(Roles = "Admin")]
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

    // Panel del cliente: solo sus propias reservas.
    [Authorize(Roles = "Cliente")]
    public IActionResult MisReservas()
    {
        if (IdClienteActual is null)
            return RedirectToAction("Index", "Home");

        var reservas = _reservaRepo.ListarPorCliente(IdClienteActual.Value);
        return View(reservas);
    }

    [HttpGet]
    public IActionResult Registrar()
    {
        bool esAdmin = User.IsInRole("Admin");

        ViewBag.EsAdmin = esAdmin;
        ViewBag.ListaCanchas = new SelectList(_canchaRepo.ListarActivas(), "IdCancha", "Nombre");
        ViewBag.ListaHorarios = new SelectList(_horarioRepo.Listar(), "IdHorario", "HoraInicio");

        var modelo = new ReservaViewModel();

        if (esAdmin)
        {
            ViewBag.ListaClientes = new SelectList(_clienteRepo.Listar(""), "IdCliente", "Nombre");
        }
        else if (IdClienteActual.HasValue)
        {
            modelo.IdCliente = IdClienteActual.Value;
        }

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Registrar(ReservaViewModel modelo)
    {
        bool esAdmin = User.IsInRole("Admin");

        // Un Cliente solo puede reservar para sí mismo, sin importar lo enviado en el form.
        if (!esAdmin)
        {
            if (IdClienteActual is null)
                return Forbid();

            modelo.IdCliente = IdClienteActual.Value;
        }

        if (!ModelState.IsValid)
        {
            ViewBag.EsAdmin = esAdmin;
            ViewBag.ListaCanchas = new SelectList(_canchaRepo.ListarActivas(), "IdCancha", "Nombre", modelo.IdCancha);
            ViewBag.ListaHorarios = new SelectList(_horarioRepo.Listar(), "IdHorario", "HoraInicio", modelo.IdHorario);

            if (esAdmin)
                ViewBag.ListaClientes = new SelectList(_clienteRepo.Listar(""), "IdCliente", "Nombre", modelo.IdCliente);

            return View(modelo);
        }

        _reservaRepo.Insertar(modelo);
        TempData["Mensaje"] = "Reserva registrada exitosamente.";

        return esAdmin
            ? RedirectToAction("Index")
            : RedirectToAction("MisReservas");
    }
    
    // ========== REPORTE POR RANGO DE FECHAS (solo Admin) ==========
    [Authorize(Roles = "Admin")]
    public IActionResult Reporte(DateTime? fechaInicio, DateTime? fechaFin, int pagina = 1)
    {
        int tamanoPagina = 5;
        int totalRegistros;

        var reservas = _reservaRepo.ListarPaginado(fechaInicio, fechaFin, pagina, tamanoPagina, out totalRegistros);

        ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
        ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
        ViewBag.PaginaActual = pagina;
        ViewBag.TotalRegistros = totalRegistros;
        ViewBag.TamanoPagina = tamanoPagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanoPagina);

        return View(reservas);
    }
}
