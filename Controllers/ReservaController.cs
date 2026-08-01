using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FulbitoBravo.Data;
using FulbitoBravo.Interfaces;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers
{
    public class ReservaController : Controller
    {
        private readonly IReservaRepositorio _reservaRepo;
        private readonly ClienteRepositorio _clienteRepo;
        private readonly CanchaRepositorio _canchaRepo;
        private readonly HorarioRepositorio _horarioRepo;

        // Inyectamos IReservaRepositorio en lugar de la clase concreta
        public ReservaController(
            IReservaRepositorio reservaRepo, 
            ClienteRepositorio clienteRepo, 
            CanchaRepositorio canchaRepo, 
            HorarioRepositorio horarioRepo)
        {
            _reservaRepo = reservaRepo;
            _clienteRepo = clienteRepo;
            _canchaRepo = canchaRepo;
            _horarioRepo = horarioRepo;
        }

        public IActionResult Index()
        {
            // Casteamos temporalmente para usar el Listar() existente
            var repoConcreto = _reservaRepo as ReservaRepositorio;
            var reservas = repoConcreto?.Listar() ?? new List<ReservaViewModel>();
            return View(reservas);
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            CargarCombos();
            return View(new ReservaViewModel { FechaReserva = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(ReservaViewModel modelo, decimal monto = 80.00m)
        {
            if (!ModelState.IsValid)
            {
                CargarCombos(modelo);
                return View(modelo);
            }

            // Ejecuta el Stored Procedure transaccional (Reserva + Pago)
            bool registrado = await _reservaRepo.RegistrarReservaConPagoAsync(
                modelo.IdCliente,
                modelo.IdCancha,
                modelo.FechaReserva,
                modelo.IdHorario,
                monto
            );

            if (registrado)
            {
                TempData["Mensaje"] = "Reserva y pago registrados exitosamente.";
                return RedirectToAction("Index");
            }

            // Si la cancha está ocupada o hubo un error transaccional
            ModelState.AddModelError("", "La cancha ya se encuentra reservada en el horario seleccionado.");
            CargarCombos(modelo);
            return View(modelo);
        }

        // Método auxiliar para no repetir código de SelectList
        private void CargarCombos(ReservaViewModel? modelo = null)
        {
            ViewBag.ListaClientes = new SelectList(_clienteRepo.Listar(""), "IdCliente", "Nombre", modelo?.IdCliente);
            ViewBag.ListaCanchas = new SelectList(_canchaRepo.Listar(), "IdCancha", "Nombre", modelo?.IdCancha);
            ViewBag.ListaHorarios = new SelectList(_horarioRepo.Listar(), "IdHorario", "HoraInicio", modelo?.IdHorario);
        }
    }
}


