using AppWeb.Client.Services;
using AppWeb.Infrastructure;
using AppWeb.Application;
using AppWeb.Components;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMudServices(); //Add MudBlazor services

builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents(); //Add support for Blazor WebAssembly components
builder.Services.AddControllersWithViews(); //Controllers & Views (optional but keeps compatibility with cookie-auth views)

var apiBase = new Uri(builder.Configuration["ApiBase"] ?? "https://localhost:5001/"); //HTTP clients for prerendered components
builder.Services.AddHttpClient<IUsersApiClient, UsersApiClient>(c => c.BaseAddress = apiBase); //suggested system more reusable

builder.Services.AddApplication(); // Application layer services and configurations
builder.Services.AddInfrastructure(builder.Configuration); //GraphQL, Repositories, etc.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) { app.UseWebAssemblyDebugging(); }
else { app.UseExceptionHandler("/Error", createScopeForErrors: true); app.UseHsts(); }

app.UseHttpsRedirection();

app.UseStaticFiles(); //Static content for MVC & Blazor
app.UseRouting(); //Routing middleware

//Authentication
app.UseAuthentication();
app.UseAuthorization();

//Middleware WASM prerendering
app.UseAntiforgery();
app.MapStaticAssets();

// Razor Components (WASM+CSR)
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(AppWeb.Client._Imports).Assembly);

//GraphQL endpoint and conventional route mapping MVC (Auth, etc.)
app.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=Login}/{id?}");
app.MapGraphQL("/graphql"); //GraphQL endpoint
app.Run();