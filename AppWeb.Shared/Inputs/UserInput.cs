using System.ComponentModel.DataAnnotations;

namespace AppWeb.Shared.Inputs;

public class CreateUserInput
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; } = string.Empty;

    [MinLength(6)]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    [MinLength(6)]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
