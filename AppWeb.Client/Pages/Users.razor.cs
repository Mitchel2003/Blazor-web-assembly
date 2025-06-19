using Microsoft.AspNetCore.Components;
using AppWeb.Client.Services;
using AppWeb.Shared.Dtos;

namespace AppWeb.Client.Pages;

public partial class Users : ComponentBase
{
    [Inject] private IUsersApiClient UsersApi { get; set; } = default!;
    protected IReadOnlyList<UserListDto>? users { get; private set; }
    protected override async Task OnInitializedAsync() => users = await UsersApi.GetUsersAsync();
}