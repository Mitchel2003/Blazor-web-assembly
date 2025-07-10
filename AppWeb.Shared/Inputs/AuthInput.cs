using System.ComponentModel.DataAnnotations;

namespace AppWeb.Shared.Inputs;

public class LoginInput
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}