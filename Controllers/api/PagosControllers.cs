using Microsoft.AspNetCore.Mvc;
using FulbitoBravo.Data;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
public class PagosController : ControllerBase
{
    private readonly PagoRepositorio _repo;

    public PagosController(PagoRepositorio repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult Listar()
    {
        var pagos = _repo.Listar();

        return Ok(pagos);
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var pago = _repo.ObtenerPorId(id);

        if (pago == null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el pago con ID {id}."
            });
        }

        return Ok(pago);
    }

    [HttpPost]
    public IActionResult Crear(PagoViewModel modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _repo.Insertar(modelo);

        return Ok(new
        {
            mensaje = "Pago registrado exitosamente."
        });
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(
        int id,
        PagoViewModel modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != modelo.IdPago)
        {
            return BadRequest(new
            {
                mensaje = "El ID de la URL no coincide con el ID del pago."
            });
        }

        var pago = _repo.ObtenerPorId(id);

        if (pago == null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el pago con ID {id}."
            });
        }

        var actualizado = _repo.Actualizar(modelo);

        if (!actualizado)
        {
            return NotFound(new
            {
                mensaje = $"No se pudo actualizar el pago con ID {id}."
            });
        }

        return Ok(new
        {
            mensaje = "Pago actualizado exitosamente."
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        var pago = _repo.ObtenerPorId(id);

        if (pago == null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el pago con ID {id}."
            });
        }

        var eliminado = _repo.Eliminar(id);

        if (!eliminado)
        {
            return NotFound(new
            {
                mensaje = $"No se pudo eliminar el pago con ID {id}."
            });
        }

        return NoContent();
    }
}