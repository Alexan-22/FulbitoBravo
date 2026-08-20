using Microsoft.AspNetCore.Mvc;
using FulbitoBravo.Data;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
public class HorariosController : ControllerBase
{
    private readonly HorarioRepositorio _repo;

    public HorariosController(HorarioRepositorio repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult Listar()
    {
        var horarios = _repo.Listar();

        return Ok(horarios);
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var horario = _repo.ObtenerPorId(id);

        if (horario == null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el horario con ID {id}."
            });
        }

        return Ok(horario);
    }

    [HttpPost]
    public IActionResult Crear(HorarioViewModel modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _repo.Insertar(modelo);

        return Ok(new
        {
            mensaje = "Horario registrado exitosamente."
        });
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(
        int id,
        HorarioViewModel modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != modelo.IdHorario)
        {
            return BadRequest(new
            {
                mensaje = "El ID de la URL no coincide con el ID del horario."
            });
        }

        var horario = _repo.ObtenerPorId(id);

        if (horario == null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el horario con ID {id}."
            });
        }

        var actualizado = _repo.Actualizar(modelo);

        if (!actualizado)
        {
            return NotFound(new
            {
                mensaje = $"No se pudo actualizar el horario con ID {id}."
            });
        }

        return Ok(new
        {
            mensaje = "Horario actualizado exitosamente."
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        var horario = _repo.ObtenerPorId(id);

        if (horario == null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el horario con ID {id}."
            });
        }

        var eliminado = _repo.Eliminar(id);

        if (!eliminado)
        {
            return NotFound(new
            {
                mensaje = $"No se pudo eliminar el horario con ID {id}."
            });
        }

        return NoContent();
    }
}