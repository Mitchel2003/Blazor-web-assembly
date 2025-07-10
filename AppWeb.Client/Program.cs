using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AppWeb.Client; //using directive for extension methods

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", optional: true, reloadOnChange: true);

// Get API base URL from configuration
var apiBaseUrl = builder.Configuration["ApiBase"] ?? builder.HostEnvironment.BaseAddress;

//Register all client services & ViewModels via extension helper for maintainability
builder.Services.AddClient(apiBase: new Uri(apiBaseUrl));

// Set the root component for the app
builder.RootComponents.Add<Routes>("#app");

await builder.Build().RunAsync();