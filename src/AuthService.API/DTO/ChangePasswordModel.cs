using System.ComponentModel.DataAnnotations;

namespace AuthService.API.DTO;

public class ChangePasswordModel
{
    [MinLength(6)] [Required] public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string RepeatPassword { get; set; } = string.Empty;
}