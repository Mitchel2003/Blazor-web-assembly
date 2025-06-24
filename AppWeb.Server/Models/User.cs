using System.ComponentModel.DataAnnotations;

namespace AppWeb.Server.Models;

/// <summary>
/// Domain model representing a user that can be listed.
/// </summary>
public class User
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; } = false;
    [Required] public string Password { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Username { get; set; } = string.Empty;
}