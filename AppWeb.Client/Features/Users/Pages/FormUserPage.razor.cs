using Microsoft.AspNetCore.Components;
using AppWeb.ViewModels.Features.Users;

namespace AppWeb.Client.Features.Users.Pages;

public partial class FormUserPage
{
    [Inject] public ICreateUserVM createVM { get; set; } = default!;
    [Inject] public IUpdateUserVM updateVM { get; set; } = default!;
    [Parameter] public int? Id { get; set; }

    protected override async Task OnInitializedAsync()
    { if (Id.HasValue) { await updateVM.LoadUserAsync(Id.Value); } }
}