using Microsoft.AspNetCore.Components;
using AppWeb.Shared.Inputs;
using MudBlazor;

namespace AppWeb.Client.Features.Users.Components;

public partial class FormUser : ComponentBase
{
    private readonly AppWeb.Shared.Validators.CreateUserInputValidator _validator = new();
    [Parameter] public EventCallback<CreateUserInput> OnValidSubmit { get; set; }
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }
    [Inject] private NavigationManager Nav { get; set; } = default!;
    private CreateUserInput _model = new();
    private MudForm _form = default!;

    private void Cancel()
    {
        if (MudDialog is not null) { MudDialog.Cancel(); }
        else { Nav.NavigateTo("/users"); }
    }

    private async Task SubmitAsync()
    {
        await _form.Validate();
        var fvResult = _validator.Validate(_model);
        if (!_form.IsValid || !fvResult.IsValid) return; //inline already shown
        if (MudDialog is not null) { MudDialog.Close(DialogResult.Ok(_model)); }
        else if (OnValidSubmit.HasDelegate) { await OnValidSubmit.InvokeAsync(_model); }
    }
}