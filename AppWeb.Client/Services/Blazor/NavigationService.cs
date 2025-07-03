using Microsoft.AspNetCore.Components;
using AppWeb.ViewModels.Core.Services;

namespace AppWeb.Client.Services.Blazor;

public class BlazorNavigationService : INavigationService
{
    private readonly NavigationManager _navigationManager;

    public BlazorNavigationService(NavigationManager navigationManager)
    { _navigationManager = navigationManager; }

    public Task GoBackAsync() { throw new NotImplementedException(); }

    public Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        _navigationManager.NavigateTo(route);
        return Task.CompletedTask;
    }
}