namespace FulbitoBravo.Models;

public class UsuarioViewModel
{
    public int IdUsuario { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Cliente"; // Admin | Cliente
    public int? IdCliente { get; set; }
    public bool Activo { get; set; } = true;
}
