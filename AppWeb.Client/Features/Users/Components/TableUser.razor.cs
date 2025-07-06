using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.ViewModels.Core.Services;
using Microsoft.AspNetCore.Components;
using CommunityToolkit.Mvvm.Input;
using AppWeb.Client.Services;
using AppWeb.Shared.Dtos;
using MudBlazor;

namespace AppWeb.Client.Features.Users.Components;

public partial class TableUserVM : ObservableObject
{
    [ObservableProperty] private IReadOnlyList<UserResultDto> _users = Array.Empty<UserResultDto>();
    [ObservableProperty] private HashSet<UserResultDto> _selectedItems = new();
    [ObservableProperty] private string _searchString = string.Empty;
    [ObservableProperty] private bool _loading;

    public EventCallback<List<UserResultDto>> OnBulkDelete;
    public EventCallback<UserResultDto> OnDelete;
    public EventCallback<UserResultDto> OnEdit;
    public EventCallback<UserResultDto> OnView;
    public EventCallback OnRefresh;
    public EventCallback OnAdd;

    private readonly INavigationService _navigationService;
    private readonly IUsersApiClient _usersApiClient;
    private readonly IDialogService _dialogService;
    private readonly ISnackbar _snackbar;

    public TableUserVM(IDialogService dialogService, IUsersApiClient usersApiClient, ISnackbar snackbar, INavigationService navigationService)
    {
        _snackbar = snackbar;
        _dialogService = dialogService;
        _usersApiClient = usersApiClient;
        _navigationService = navigationService;
    }

    /// <summary>Lista filtrada según el texto de búsqueda.</summary>
    public IEnumerable<UserResultDto> FilteredUsers => string.IsNullOrWhiteSpace(SearchString)
        ? Users : Users.Where(u =>
            (u.Username?.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ?? false)
            || (u.Email?.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ?? false));

    [RelayCommand]
    public void Add() => OnAdd.InvokeAsync();

    [RelayCommand]
    public void Edit(UserResultDto user) => OnEdit.InvokeAsync(user);

    [RelayCommand]
    public void Delete(UserResultDto user) => OnDelete.InvokeAsync(user);

    [RelayCommand]
    public void View(UserResultDto user) => OnView.InvokeAsync(user);

    [RelayCommand]
    public void Refresh() => OnRefresh.InvokeAsync();

    [RelayCommand]
    public void BulkDelete()
    { if (SelectedItems.Count > 0) { OnBulkDelete.InvokeAsync(SelectedItems.ToList()); } }
}