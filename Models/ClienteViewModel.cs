using System.ComponentModel.DataAnnotations;

namespace FulbitoBravo.Models;

public class ClienteViewModel
{
    public int IdCliente { get; set; }

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
}
