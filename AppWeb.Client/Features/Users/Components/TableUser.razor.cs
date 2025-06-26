using Microsoft.AspNetCore.Components;
using AppWeb.Shared.Dtos;

namespace AppWeb.Client.Features.Users.Components;

public partial class TableUser : ComponentBase
{
    /// <summary>Lista de usuarios a renderizar.</summary>
    [Parameter] public IReadOnlyList<UserResultDto> Users { get; set; } = new List<UserResultDto>();

    /// <summary>Evento que se dispara cuando el botón "Add" es presionado.</summary>
    [Parameter] public EventCallback<UserResultDto> OnDelete { get; set; }
    [Parameter] public EventCallback<UserResultDto> OnEdit { get; set; }
    [Parameter] public EventCallback<UserResultDto> OnView { get; set; }
    [Parameter] public EventCallback OnAdd { get; set; }
    private string _searchString = string.Empty;

    /// <summary>Lista filtrada según el texto de búsqueda.</summary>
    protected IEnumerable<UserResultDto> FilteredUsers => string.IsNullOrWhiteSpace(_searchString)
        ? Users
        : Users.Where(u =>
            (u.Username?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false)
            || (u.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false));

    protected async Task OnAddClicked()
    { if (OnAdd.HasDelegate) await OnAdd.InvokeAsync(); }

    private async Task ViewClicked(UserResultDto user)
    { if (OnView.HasDelegate) await OnView.InvokeAsync(user); }

    private async Task EditClicked(UserResultDto user)
    { if (OnEdit.HasDelegate) await OnEdit.InvokeAsync(user); }

    private async Task DeleteClicked(UserResultDto user)
    { if (OnDelete.HasDelegate) await OnDelete.InvokeAsync(user); }
}