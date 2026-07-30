namespace FulbitoBravo.Models;

public class ReservaViewModel
{
    public int IdReserva { get; set; }
    public int IdCliente { get; set; }
    public string? NombreCliente { get; set; }
    public int IdCancha { get; set; }
    public string? NombreCancha { get; set; }
    public DateTime FechaReserva { get; set; }
    public int IdHorario { get; set; }
    public string? HorarioTexto { get; set; }
    public string EstadoReserva { get; set; } = "Confirmada";
}
