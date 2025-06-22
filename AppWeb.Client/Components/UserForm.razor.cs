using Microsoft.AspNetCore.Components;
using AppWeb.Shared.Inputs;
using MudBlazor;

namespace AppWeb.Client.Components;

public partial class UserForm : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }
    [Parameter] public EventCallback<CreateUserInput> OnValidSubmit { get; set; }
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
        if (!_form.IsValid) { return; }
        if (MudDialog is not null) { MudDialog.Close(DialogResult.Ok(_model)); }
        else if (OnValidSubmit.HasDelegate) { await OnValidSubmit.InvokeAsync(_model); }
    }
}