using System.ComponentModel.DataAnnotations;

namespace CRM.ViewModels;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A password deve ter pelo menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
    [Display(Name = "Confirmar Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}