namespace AppWeb.ViewModels.Features.Users;

/// <summary>Event arguments for when a user is created.</summary>
public class UserCreatedEventArgs : UserEventArgs { }

/// <summary>Event arguments for when a user is updated.</summary>
public class UserUpdatedEventArgs : UserEventArgs { }

/// <summary>Base class for user-related event arguments.</summary>
public abstract class UserEventArgs : EventArgs
{
    /// <summary>Indicates if the operation was successful.</summary>
    public bool Success { get; set; }

    /// <summary>The ID of the user.</summary>
    public int UserId { get; set; }

    /// <summary>The username of the user.</summary>
    public string? Username { get; set; }

    /// <summary>Indicates if navigation should occur after the event.</summary>
    public bool ShouldNavigate { get; set; }
}