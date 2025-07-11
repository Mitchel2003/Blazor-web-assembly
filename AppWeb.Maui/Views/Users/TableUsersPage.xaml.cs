using AppWeb.ViewModels.Features.Contracts;
using AppWeb.ViewModels.Features.Users;
using AppWeb.Maui.Views.Common;
using AppWeb.Shared.Dtos;

namespace AppWeb.Maui.Views.Users;

public partial class TableUsersPage : BaseContentPage<ITableUsersPageVM>
{
    public TableUsersPage(ITableUsersPageVM viewModel) : base(viewModel)
    {
        InitializeComponent();

        // Subscribe to ViewModel events
        ViewModel.UserSelected += OnUserSelected;
        ViewModel.UserDeleted += OnUserDeleted;
    }

    protected override async Task OnViewModelAppearing()
    {
        await ViewModel.LoadAsync();
        await base.OnViewModelAppearing();
    }

    private void OnUserSelected(object sender, UserResultDto e)
    {
        //Handle user selected event
        //Navigation is typically handled by the ViewModel via NavigationService
    }

    private void OnUserDeleted(object sender, UserResultDto e)
    {
        //Handle user deleted event
        //Refresh may be handled by the ViewModel
    }

    protected override void OnDisappearing()
    {
        // Unsubscribe from events to prevent memory leaks
        ViewModel.UserSelected -= OnUserSelected;
        ViewModel.UserDeleted -= OnUserDeleted;
        base.OnDisappearing();
    }
}