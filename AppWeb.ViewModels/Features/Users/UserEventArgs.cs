using AppWeb.Shared.Dtos;

namespace AppWeb.ViewModels.Features.Users;

#region Form ------------------------------------------------------------
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
#endregion ---------------------------------------------------------------------

#region Table ------------------------------------------------------------
/// <summary>Event arguments for table operations.</summary>
public class TableOperationEventArgs : EventArgs
{
    /// <summary>Gets or sets the operation type.</summary>
    public TableOperation Operation { get; set; }

    /// <summary>Gets or sets the affected items.</summary>
    public List<UserResultDto>? Items { get; set; }
}

/// <summary>Table operation types.</summary>
public enum TableOperation
{
    /// <summary>Add operation.</summary>
    Add,
    /// <summary>Edit operation.</summary>
    Edit,
    /// <summary>Delete operation.</summary>
    Delete,
    /// <summary>View operation.</summary>
    View,
    /// <summary>Refresh operation.</summary>
    Refresh,
    /// <summary>Bulk delete operation.</summary>
    BulkDelete
}

/// <summary>Event arguments for table user events.</summary>
public class TableUserEventArgs : EventArgs
{
    /// <summary>Gets or sets the user associated with the event.</summary>
    public UserResultDto? User { get; set; }
}
#endregion ---------------------------------------------------------------------