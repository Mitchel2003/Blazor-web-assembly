using Microsoft.AspNetCore.Components;
using AppWeb.Shared.Dtos;

namespace AppWeb.Client.Features.Users.Components;

public partial class TableUser : ComponentBase
{
    /// <summary>Lista de usuarios a renderizar.</summary>
    [Parameter] public IReadOnlyList<UserListDto> Users { get; set; } = new List<UserListDto>();

    /// <summary>Evento que se dispara cuando el botón "Add" es presionado.</summary>
    [Parameter] public EventCallback OnAdd { get; set; }
    private string _searchString = string.Empty;

    /// <summary>Lista filtrada según el texto de búsqueda.</summary>
    protected IEnumerable<UserListDto> FilteredUsers => string.IsNullOrWhiteSpace(_searchString)
        ? Users
        : Users.Where(u =>
            (u.Username?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (u.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false));

    protected async Task OnAddClicked()
    { if (OnAdd.HasDelegate) { await OnAdd.InvokeAsync(null); } }
}