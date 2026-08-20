namespace FulbitoBravo.Models;

public class CanchaViewModel
{
    public int IdCancha { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Estado { get; set; } = true;
}
