using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using CommunityToolkit.Mvvm.Input;
using FluentValidation.Results;
using FluentValidation;
using MudBlazor;

using AppWeb.Shared.Validators;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;

namespace AppWeb.Client.Features.Users.Components;

public partial class FormUserVM : ObservableObject
{
    [ObservableProperty] private UpdateUserInput _updateModel = new();
    [ObservableProperty] private CreateUserInput _createModel = new();
    [ObservableProperty] private UserResultDto? _existing;
    [ObservableProperty] private bool _isActive = true;
    [ObservableProperty] private bool _success = false;
    [ObservableProperty] private bool _loading = false;

    public bool IsEdit => Existing is not null;
    public IMudDialogInstance? MudDialog { get; set; }
    private readonly CreateUserInputValidator _createValidator = new();
    private readonly UpdateUserInputValidator _updateValidator = new();
    private readonly NavigationManager _nav;

    #region Current ------------------------------------------------------------
    public EventCallback<object> OnValidSubmit { get; set; }
    public object CurrentModel => IsEdit ? (object)UpdateModel : CreateModel;
    public IValidator CurrentValidator => IsEdit ? (IValidator)_updateValidator : _createValidator;
    #endregion ---------------------------------------------------------------------

    public FormUserVM(NavigationManager nav)
    { _nav = nav; }

    public void Initialize(UserResultDto? existingUser = null)
    {
        Existing = existingUser;
        if (IsEdit && Existing is not null)
        {
            IsActive = Existing.IsActive;
            UpdateModel.Id = Existing.Id;
            UpdateModel.Email = Existing.Email;
            UpdateModel.Username = Existing.Username;
            UpdateModel.Password = Existing.Password;
        }
        else { IsActive = true; }
    }

    [RelayCommand]
    public void Cancel()
    {
        if (MudDialog is not null) MudDialog.Cancel();
        else _nav.NavigateTo("/users");
    }

    [RelayCommand]
    public async Task Submit(MudForm form)
    {
        if (form == null) return;
        await form.Validate();
        ValidationResult fvResult = IsEdit
            ? _updateValidator.Validate(UpdateModel)
            : _createValidator.Validate(CreateModel);
        if (!form.IsValid || !fvResult.IsValid) return;
        object payload = IsEdit ? UpdateModel : CreateModel;

        Loading = true; //Set loading state
        //Apply the IsActive state to the model
        if (IsEdit) { UpdateModel.IsActive = IsActive; }
        else { CreateModel.IsActive = IsActive; }
        //Invoke the OnValidSubmit callback if it has delegates
        if (MudDialog is not null) MudDialog.Close(DialogResult.Ok(payload));
        else if (OnValidSubmit.HasDelegate) await OnValidSubmit.InvokeAsync(payload);
        Loading = false;
        Success = true;
    }
}