using Microsoft.AspNetCore.Components;
using AppWeb.Client.Services;
using AppWeb.Shared.Dtos;
using MudBlazor;

namespace AppWeb.Client.Features.Users.Components;

public partial class TableUser : ComponentBase
{
    [Inject] private IDialogService DialogService { get; set; } = default!;
    [Inject] private IUsersApiClient UsersApiClient { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    [Parameter] public IReadOnlyList<UserResultDto> Users { get; set; } = Array.Empty<UserResultDto>();
    [Parameter] public EventCallback<List<UserResultDto>> OnBulkDelete { get; set; }
    [Parameter] public EventCallback<UserResultDto> OnDelete { get; set; }
    [Parameter] public EventCallback<UserResultDto> OnEdit { get; set; }
    [Parameter] public EventCallback<UserResultDto> OnView { get; set; }
    [Parameter] public EventCallback OnRefresh { get; set; }
    [Parameter] public EventCallback OnAdd { get; set; }
    [Parameter] public bool Loading { get; set; }

    private HashSet<UserResultDto> _selectedItems = new();
    private string _searchString = string.Empty;

    /// <summary>Lista filtrada según el texto de búsqueda.</summary>
    protected IEnumerable<UserResultDto> FilteredUsers => string.IsNullOrWhiteSpace(_searchString)
        ? Users : Users.Where(u =>
            (u.Username?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false)
            || (u.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false));

    protected override async Task OnInitializedAsync()
    { await base.OnInitializedAsync(); }

    private void OnAddClicked()
    { OnAdd.InvokeAsync(); }

    private void EditClicked(UserResultDto user)
    { OnEdit.InvokeAsync(user); }

    private void DeleteClicked(UserResultDto user)
    { OnDelete.InvokeAsync(user); }

    private void ViewClicked(UserResultDto user)
    { OnView.InvokeAsync(user); }

    private void RefreshData()
    { OnRefresh.InvokeAsync(); }

    private void BulkDeleteClicked()
    { if (_selectedItems.Count > 0) { OnBulkDelete.InvokeAsync(_selectedItems.ToList()); } }
}