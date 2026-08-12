using Microsoft.AspNetCore.Mvc;
using FulbitoBravo.Data;
using FulbitoBravo.Models;

namespace FulbitoBravo.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ClienteRepositorio _repo;

    public ClientesController(ClienteRepositorio repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult Listar(
        string? buscar = null,
        int pagina = 1,
        int tamanoPagina = 5)
    {
        if (pagina < 1)
            return BadRequest(new
            {
                mensaje = "La página debe ser mayor o igual a 1."
            });

        if (tamanoPagina < 1)
            return BadRequest(new
            {
                mensaje = "El tamaño de página debe ser mayor o igual a 1."
            });

        var clientes = _repo.ListarPaginado(
            buscar,
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
            clientes
        });
    }

    // GET: api/Clientes/5
    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var cliente = _repo.ObtenerPorId(id);

        if (cliente == null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el cliente con ID {id}."
            });
        }

        return Ok(cliente);
    }

    [HttpPost]
    public IActionResult Crear(ClienteViewModel modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _repo.Insertar(modelo);

        return Ok(new
        {
            mensaje = $"Cliente '{modelo.Nombre} {modelo.Apellido}' registrado exitosamente."
        });
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(
        int id,
        ClienteViewModel modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != modelo.IdCliente)
        {
            return BadRequest(new
            {
                mensaje = "El ID de la URL no coincide con el ID del cliente."
            });
        }

        var cliente = _repo.ObtenerPorId(id);

        if (cliente == null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el cliente con ID {id}."
            });
        }

        var actualizado = _repo.Actualizar(modelo);

        if (!actualizado)
        {
            return NotFound(new
            {
                mensaje = $"No se pudo actualizar el cliente con ID {id}."
            });
        }

        return Ok(new
        {
            mensaje = $"Cliente '{modelo.Nombre} {modelo.Apellido}' actualizado exitosamente."
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        var cliente = _repo.ObtenerPorId(id);

        if (cliente == null)
        {
            return NotFound(new
            {
                mensaje = $"No se encontró el cliente con ID {id}."
            });
        }

        var eliminado = _repo.Eliminar(id);

        if (!eliminado)
        {
            return NotFound(new
            {
                mensaje = $"No se pudo eliminar el cliente con ID {id}."
            });
        }

        return NoContent();
    }
}