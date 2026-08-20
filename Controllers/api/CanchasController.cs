using Microsoft.AspNetCore.Mvc;
using FulbitoBravo.Data;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
public class CanchasController : ControllerBase
{
    private readonly CanchaRepositorio _repo;

    public CanchasController(CanchaRepositorio repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult Listar()
    {
        var canchas = _repo.Listar();

        return Ok(canchas);
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var cancha = _repo.ObtenerPorId(id);

        if (cancha == null)
            return NotFound(new
            {
                mensaje = $"No se encontró la cancha con ID {id}."
            });

        return Ok(cancha);
    }

    [HttpPost]
    public IActionResult Crear(CanchaViewModel modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _repo.Insertar(modelo);

        return Ok(new
        {
            mensaje = $"Cancha '{modelo.Nombre}' registrada exitosamente."
        });
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(int id, CanchaViewModel modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != modelo.IdCancha)
        {
            return BadRequest(new
            {
                mensaje = "El ID de la URL no coincide con el ID de la cancha."
            });
        }

        var cancha = _repo.ObtenerPorId(id);

        if (cancha == null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la cancha con ID {id}."
            });
        }

        var actualizado = _repo.Actualizar(modelo);

        if (!actualizado)
        {
            return NotFound(new
            {
                mensaje = $"No se pudo actualizar la cancha con ID {id}."
            });
        }

        return Ok(new
        {
            mensaje = $"Cancha '{modelo.Nombre}' actualizada exitosamente."
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        var cancha = _repo.ObtenerPorId(id);

        if (cancha == null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró la cancha con ID {id}."
            });
        }

        var eliminado = _repo.Eliminar(id);

        if (!eliminado)
        {
            return NotFound(new
            {
                mensaje = $"No se pudo eliminar la cancha con ID {id}."
            });
        }

        return NoContent();
    }
}