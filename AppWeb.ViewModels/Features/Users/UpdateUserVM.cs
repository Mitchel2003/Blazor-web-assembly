using CommunityToolkit.Mvvm.ComponentModel;
using AppWeb.ViewModels.Core.Services;
using AppWeb.ViewModels.Core.Factory;
using CommunityToolkit.Mvvm.Input;
using AppWeb.ViewModels.Core.Base;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;

namespace AppWeb.ViewModels.Features.Users;

/// <summary>ViewModel for user updates, implementing CRUD operations.</summary>
public partial class UpdateUserVM : ViewModelCrud<UpdateUserInput, int>, IUpdateUserVM
{
    private readonly IUsersService _usersService;
    private readonly IMessageService _messageService;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private bool _redirectionInProgress;
    public event EventHandler<UserUpdatedEventArgs>? UserUpdated;

    public UpdateUserVM(IUsersService usersService, IMessageService messageService, INavigationService navigationService, IModelFactory modelFactory)
        : base(modelFactory, messageService)
    {
        Title = "Edit User";
        _usersService = usersService;
        _messageService = messageService;
        _navigationService = navigationService;
    }

    /// <summary>Update title when model is loaded.</summary>
    protected override void OnModelLoaded()
    { base.OnModelLoaded(); Title = $"Edit User: {Model?.Username}"; }

    /// <summary>Gets the user by ID from the service.</summary>
    public override async Task<UpdateUserInput> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _usersService.GetUserByIdAsync(id, cancellationToken);
        if (user == null) return null!; //if user not found, return null
        //map to input using ModelFactory (no direct instantiation)
        return _modelFactory.CreateFrom<UpdateUserInput, UserResultDto>(user);
    }

    /// <summary>Process the save operation with the users service.</summary>
    protected override async Task<bool> OnSaveAsync()
    {
        try
        {
            var result = await _usersService.UpdateUserAsync(Model);
            if (result == null) { await _messageService.ShowErrorAsync("Error updating user"); return false; }
            await _messageService.ShowSuccessAsync("User updated successfully");
            UserUpdated?.Invoke(this, new UserUpdatedEventArgs //notify subscribers
            { Success = true, UserId = result.Id, Username = result.Username ?? Model.Username, ShouldNavigate = true });

            RedirectionInProgress = true; //initiate redirection
            await Task.Delay(800); //to allow the success message
            await NavigateToUsersList(); //navigate to users list
            return result != null;
        }
        catch (Exception ex) { await _messageService.ShowErrorAsync($"Unexpected error: {ex.Message}"); return false; }
    }

    [RelayCommand]
    /// <summary>Navigate back with confirmation if changes are pending.</summary>
    public async Task NavigateBackAsync()
    {
        if (RedirectionInProgress) return;
        if (IsModified)
        { //check for unsaved changes
            var confirm = await _messageService.ConfirmAsync("Discard Changes", "You have unsaved changes. Are you sure you want to discard them?");
            if (!confirm) return;
        }
        RedirectionInProgress = true;
        UserUpdated?.Invoke(this, new UserUpdatedEventArgs
        { Success = false, ShouldNavigate = true, UserId = ModelId });
        await NavigateToUsersList(); //navigate to users list
    }

    #region Helpers ------------------------------------------------------------
    /// <summary>Navigate to the users list.</summary>
    private async Task NavigateToUsersList()
    { await _navigationService.NavigateToAsync(new NavigationConfig(NavigationConfig.Routes.Users)); }

    /// <summary>Shows validation errors as a formatted message.</summary>
    private async void ShowMessageErrorValidation()
    {
        var errors = GetAllErrors(); //get validation error in context
        if (errors.Count > 0) await _messageService.ShowErrorAsync(string.Join("\n", errors));
    }
    #endregion ---------------------------------------------------------------------
}