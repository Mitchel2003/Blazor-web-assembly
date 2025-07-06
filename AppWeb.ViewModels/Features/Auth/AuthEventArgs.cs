namespace AppWeb.ViewModels.Features.Auth;

/// <summary>Event arguments for login success events.</summary>
public class LoginSuccessEventArgs : EventArgs
{
    /// <summary>The JWT token received after successful login.</summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>The user's display name.</summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>The user's unique identifier.</summary>
    public int UserId { get; set; }
    
    /// <summary>Indica si se debe navegar después del evento.</summary>
    public bool ShouldNavigate { get; set; }
}

/// <summary>Event arguments for logout completion events.</summary>
public class LogoutCompletedEventArgs : EventArgs
{
    /// <summary>Indicates if the logout was successful.</summary>
    public bool Success { get; set; }
    
    /// <summary>Error message if logout failed.</summary>
    public string? ErrorMessage { get; set; }
}