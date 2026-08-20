using Microsoft.AspNetCore.Mvc;
using FulbitoBravo.Data;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
public class ReservasController : ControllerBase
{
    private readonly ReservaRepositorio _repo;

    public ReservasController(ReservaRepositorio repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult Listar(
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        int pagina = 1,
        int tamanoPagina = 5)
    {
        if (pagina < 1)
        {
            return BadRequest(new
            {
                mensaje = "La página debe ser mayor o igual a 1."
            });
        }

        if (tamanoPagina < 1)
        {
            return BadRequest(new
            {
                mensaje =
                    "El tamaño de página debe ser mayor o igual a 1."
            });
        }

        var reservas = _repo.ListarPaginado(
            fechaInicio,
            fechaFin,
            pagina,
            tamanoPagina,
            out int totalRegistros
        );

        return Ok(new
        {
            pagina,
            tamanoPagina,
            totalRegistros,
            totalPaginas = (int)Math.Ceiling(
                (double)totalRegistros / tamanoPagina
            ),
            reservas
        });
    }

    // GET: api/Reservas/1
    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var reserva = _repo.ObtenerPorId(id);

        if (reserva == null)
        {
            return NotFound(new
            {
                mensaje =
                    $"No se encontró la reserva con ID {id}."
            });
        }

        return Ok(reserva);
    }

    [HttpPost]
    public IActionResult Crear(ReservaViewModel modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _repo.Insertar(modelo);

        return Ok(new
        {
            mensaje = "Reserva registrada exitosamente."
        });
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(
        int id,
        ReservaViewModel modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != modelo.IdReserva)
        {
            return BadRequest(new
            {
                mensaje =
                    "El ID de la URL no coincide con el ID de la reserva."
            });
        }

        var reserva = _repo.ObtenerPorId(id);

        if (reserva == null)
        {
            return NotFound(new
            {
                mensaje =
                    $"No se encontró la reserva con ID {id}."
            });
        }

        var actualizado = _repo.Actualizar(modelo);

        if (!actualizado)
        {
            return NotFound(new
            {
                mensaje =
                    $"No se pudo actualizar la reserva con ID {id}."
            });
        }

        return Ok(new
        {
            mensaje = "Reserva actualizada exitosamente."
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        var reserva = _repo.ObtenerPorId(id);

        if (reserva == null)
        {
            return NotFound(new
            {
                mensaje =
                    $"No se encontró la reserva con ID {id}."
            });
        }

        var eliminado = _repo.Eliminar(id);

        if (!eliminado)
        {
            return NotFound(new
            {
                mensaje =
                    $"No se pudo eliminar la reserva con ID {id}."
            });
        }

        return NoContent();
    }
}