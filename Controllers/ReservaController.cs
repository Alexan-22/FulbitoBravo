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

        var modelo = new ReservaViewModel
        {
            FechaReserva = DateTime.Today
        };

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
        if (modelo.FechaReserva.Date < DateTime.Today)
        {
            ModelState.AddModelError("FechaReserva", "La fecha de la reserva no puede ser anterior a hoy.");
        }
        if (_reservaRepo.ExisteReservaActiva(modelo.IdCancha, modelo.FechaReserva, modelo.IdHorario))
        {
            ModelState.AddModelError("", "Ya existe una reserva confirmada o en curso para esa cancha, fecha y horario. Por favor, elegir otra.");
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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult EditarEstado(int id)
    {
        var reserva = _reservaRepo.ObtenerPorId(id);

        if (reserva == null)
            return NotFound();

        // Solo se pueden editar reservas de hoy o futuras
        if (reserva.FechaReserva.Date < DateTime.Today)
        {
            TempData["Mensaje"] = "No se puede editar una reserva de una fecha pasada.";
            return RedirectToAction("Index");
        }

        return View(reserva);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditarEstado(int id, string estadoReserva)
    {
        var reserva = _reservaRepo.ObtenerPorId(id);

        if (reserva == null)
            return NotFound();

        if (reserva.FechaReserva.Date < DateTime.Today)
        {
            TempData["Mensaje"] = "No se puede editar una reserva de una fecha pasada.";
            return RedirectToAction("Index");
        }

        if (string.IsNullOrWhiteSpace(estadoReserva))
        {
            ModelState.AddModelError("", "Debe seleccionar un estado.");
            return View(reserva);
        }

        _reservaRepo.ActualizarEstado(id, estadoReserva);
        TempData["Mensaje"] = "Estado de la reserva actualizado correctamente.";
        return RedirectToAction("Index");
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
