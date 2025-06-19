using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AppWeb.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//Configure HttpClient pointing to server origin
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IUsersApiClient, UsersApiClient>();

await builder.Build().RunAsync();