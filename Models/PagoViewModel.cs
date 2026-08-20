namespace FulbitoBravo.Models;

public class PagoViewModel
{
    public int IdPago { get; set; }
    public int IdReserva { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; } = DateTime.Now;
    public string EstadoPago { get; set; } = "Pagado";
}
