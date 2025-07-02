using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AppWeb.Client; //using directive for extension methods

var builder = WebAssemblyHostBuilder.CreateDefault(args);
//Register all client services & ViewModels via extension helper for maintainability
builder.Services.AddClient(apiBase: new Uri(builder.HostEnvironment.BaseAddress));

await builder.Build().RunAsync();