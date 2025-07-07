using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.ViewModels.Core.Services;
using CommunityToolkit.Mvvm.Input;
using AppWeb.ViewModels.Core.Base;
using AppWeb.Shared.Dtos;

namespace AppWeb.ViewModels.Features.Users;

/// <summary>ViewModel for displaying and managing users in a table.</summary>
public partial class TableUserVM : ViewModelBase, ITableUserVM
{
    private readonly IUsersService _usersService;
    private readonly IMessageService _messageService;

    [ObservableProperty] private IReadOnlyList<UserResultDto> _users = Array.Empty<UserResultDto>();
    [ObservableProperty] private HashSet<UserResultDto> _selectedItems = new();
    [ObservableProperty] private string _searchString = string.Empty;

    public event EventHandler? OnAdd;
    public event EventHandler? OnRefresh;
    public event EventHandler<UserResultDto>? OnEdit;
    public event EventHandler<UserResultDto>? OnDelete;
    public event EventHandler<UserResultDto>? OnView;
    public event EventHandler<List<UserResultDto>>? OnBulkDelete;

    public TableUserVM(IUsersService usersService, IMessageService messageService)
    {
        _usersService = usersService;
        _messageService = messageService;
        Title = "Users";
    }

    /// <summary>Filtered users based on the search string.</summary>
    public IEnumerable<UserResultDto> FilteredUsers => string.IsNullOrWhiteSpace(SearchString)
        ? Users : Users.Where(u =>
            (u.Username?.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ?? false)
            || (u.Email?.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ?? false));

    public bool IsLoading => throw new NotImplementedException();

    /// <summary>Loads the users data.</summary>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        try { IsBusy = true; Users = await _usersService.GetUsersAsync(cancellationToken); }
        catch (Exception ex) { ErrorMessage = $"Failed to load users: {ex.Message}"; await _messageService.ShowErrorAsync(ErrorMessage); }
        finally { IsBusy = false; }
    }

    /// <summary>Initialize the ViewModel.</summary>
    protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
    { await LoadAsync(cancellationToken); }

    [RelayCommand]
    private void Add()
    { OnAdd?.Invoke(this, EventArgs.Empty); }

    [RelayCommand]
    private void Edit(UserResultDto user)
    { OnEdit?.Invoke(this, user); }

    [RelayCommand]
    private void Delete(UserResultDto user)
    { OnDelete?.Invoke(this, user); }

    [RelayCommand]
    private void View(UserResultDto user)
    { OnView?.Invoke(this, user); }

    [RelayCommand]
    private void Refresh()
    { OnRefresh?.Invoke(this, EventArgs.Empty); }

    [RelayCommand]
    private void BulkDelete()
    {
        if (SelectedItems.Count > 0)
        { OnBulkDelete?.Invoke(this, SelectedItems.ToList()); }
    }
} 