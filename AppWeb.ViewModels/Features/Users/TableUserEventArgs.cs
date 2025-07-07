using AppWeb.Shared.Dtos;

namespace AppWeb.ViewModels.Features.Users;

/// <summary>Event arguments for table operations.</summary>
public class TableUserLoadedEventArgs : EventArgs
{
    /// <summary>The number of users loaded.</summary>
    public int Count { get; set; }

    /// <summary>Indicates if the loading operation was successful.</summary>
    public bool Success { get; set; }
    
    /// <summary>Any error message that occurred during loading.</summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>Event arguments for bulk delete operations.</summary>
public class BulkDeleteEventArgs : EventArgs
{
    /// <summary>Indicates if the operation was successful.</summary>
    public bool Success { get; set; }
    
    /// <summary>Number of successfully deleted users.</summary>
    public int SuccessCount { get; set; }
    
    /// <summary>Number of failed deletions.</summary>
    public int FailedCount { get; set; }
    
    /// <summary>Any error messages that occurred during the operation.</summary>
    public List<string> ErrorMessages { get; set; } = new();
} 