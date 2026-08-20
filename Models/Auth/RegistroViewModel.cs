using System.ComponentModel.DataAnnotations;

namespace FulbitoBravo.Models.Auth;

// Autorregistro público: crea al mismo tiempo la ficha de Cliente
// y la cuenta de Usuario (Rol = Cliente) enlazada a esa ficha.
public class RegistroViewModel
{
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe contener solo 8 dígitos numéricos.")]
    [Display(Name = "DNI")]
    public string DNI { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(20)]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Correo no válido.")]
    [StringLength(100)]
    public string? Correo { get; set; }

    [Required(ErrorMessage = "Elige un nombre de usuario.")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "El usuario debe tener entre 4 y 50 caracteres.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa una contraseña.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma tu contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
