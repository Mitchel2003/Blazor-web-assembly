using CommunityToolkit.Mvvm.Input;
using AppWeb.Shared.Dtos;

namespace AppWeb.ViewModels.Features.Contracts;

/// <summary>Interface for the TableUserVM.</summary>
public interface ITableUserVM : IDisposable
{
    /// <summary>Indicates if the view model is currently loading data.</summary>
    bool IsLoading { get; set; }

    /// <summary>Search string for filtering users.</summary>
    string SearchString { get; set; }

    /// <summary>Selected users in the table.</summary>
    HashSet<UserResultDto> SelectedItems { get; set; }

    /// <summary>List of users to display in the table.</summary>
    IReadOnlyList<UserResultDto> Users { get; set; }

    /// <summary>Filtered users based on search string.</summary>
    IEnumerable<UserResultDto> FilteredUsers { get; }

    /// <summary>Command to add a new user.</summary>
    IRelayCommand AddCommand { get; }

    /// <summary>Command to edit a user.</summary>
    IRelayCommand<UserResultDto> EditCommand { get; }

    /// <summary>Command to delete a user.</summary>
    IRelayCommand<UserResultDto> DeleteCommand { get; }

    /// <summary>Command to view a user's details.</summary>
    IRelayCommand<UserResultDto> ViewCommand { get; }

    /// <summary>Command to refresh the user list.</summary>
    IRelayCommand RefreshCommand { get; }

    /// <summary>Command to delete multiple selected users.</summary>
    IRelayCommand BulkDeleteCommand { get; }

    /// <summary>Loads the users data.</summary>
    Task LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>Event raised when an add action is requested.</summary>
    event EventHandler OnAdd;

    /// <summary>Event raised when a refresh is requested.</summary>
    event EventHandler OnRefresh;

    /// <summary>Event raised when an edit action is requested.</summary>
    event EventHandler<UserResultDto> OnEdit;

    /// <summary>Event raised when a delete action is requested.</summary>
    event EventHandler<UserResultDto> OnDelete;

    /// <summary>Event raised when a view action is requested.</summary>
    event EventHandler<UserResultDto> OnView;

    /// <summary>Event raised when a bulk delete action is requested.</summary>
    event EventHandler<List<UserResultDto>> OnBulkDelete;
}