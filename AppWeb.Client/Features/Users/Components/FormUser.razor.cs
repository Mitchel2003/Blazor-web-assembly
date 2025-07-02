using Microsoft.AspNetCore.Components;
using AppWeb.Shared.Validators;
using FluentValidation.Results;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;
using FluentValidation;
using MudBlazor;

namespace AppWeb.Client.Features.Users.Components;

public partial class FormUser : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }
    [Parameter] public EventCallback<object> OnValidSubmit { get; set; }
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Parameter] public UserResultDto? Existing { get; set; }

    private readonly CreateUserInputValidator _createValidator = new();
    private readonly UpdateUserInputValidator _updateValidator = new();
    private readonly CreateUserInput _createModel = new();
    private readonly UpdateUserInput _updateModel = new();
    private bool _isActive { get; set; } = false;
    private MudForm _form = default!;
    private bool _success = false;
    private bool _loading = false;
    private bool _isEdit;

    #region Context ------------------------------------------------------------
    private IValidator CurrentValidator => _isEdit ? (IValidator)_updateValidator : _createValidator;
    private object CurrentModel => _isEdit ? (object)_updateModel : _createModel;
    #endregion ---------------------------------------------------------------------

    private void Cancel()
    {
        if (MudDialog is not null) MudDialog.Cancel();
        else Nav.NavigateTo("/users");
    }

    protected override void OnInitialized()
    {
        _isEdit = Existing is not null;
        if (_isEdit && Existing is not null)
        {
            _isActive = Existing.IsActive;
            _updateModel.Id = Existing.Id;
            _updateModel.Email = Existing.Email;
            _updateModel.Username = Existing.Username;
            _updateModel.Password = Existing.Password;
        }
        else { _isActive = true; }
    }

    private async Task SubmitAsync()
    {
        await _form.Validate();
        ValidationResult fvResult = _isEdit
            ? _updateValidator.Validate(_updateModel)
            : _createValidator.Validate(_createModel);
        if (!_form.IsValid || !fvResult.IsValid) return;
        object payload = _isEdit ? _updateModel : _createModel;
        // Invoke the OnValidSubmit callback if it has delegates
        if (MudDialog is not null) MudDialog.Close(DialogResult.Ok(payload));
        else if (OnValidSubmit.HasDelegate) await OnValidSubmit.InvokeAsync(payload);
    }
}