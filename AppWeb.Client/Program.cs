using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AppWeb.Client.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//Configure HttpClient pointing to server origin
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<AppWeb.Client.Features.Users.ViewModels.UsersPageViewModel>();
builder.Services.AddScoped<IUsersApiClient, UsersApiClient>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();