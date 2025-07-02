using AppWeb.Server.Components;
using AppWeb.Client.Services;
using AppWeb.Infrastructure;
using AppWeb.Application;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>() //Enables user-secrets
    .AddEnvironmentVariables();

builder.Services.AddMudServices(); //Add MudBlazor services
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
builder.Services.AddControllersWithViews(); //keeps compatibility cookie-auth

var apiBase = new Uri(builder.Configuration["ApiBase"] ?? "https://localhost:5001/"); //HTTP clients for prerendered components
builder.Services.AddHttpClient<IUsersApiClient, UsersApiClient>(c => c.BaseAddress = apiBase); //suggested system more reusable

builder.Services.AddApplication(); // Application layer services and configurations
builder.Services.AddInfrastructure(builder.Configuration); //GraphQL, Repositories, etc.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) { app.UseWebAssemblyDebugging(); }
else { app.UseExceptionHandler("/Error", createScopeForErrors: true); app.UseHsts(); }

app.UseHttpsRedirection();
app.UseStaticFiles(); //Static content (wwwroot) first
app.UseRouting(); //Routing middleware

//Authentication
app.UseAuthentication();
app.UseAuthorization();

//Middleware WASM prerendering
app.UseAntiforgery();
app.MapStaticAssets();

//Razor Components (WASM+CSR)
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(AppWeb.Client._Imports).Assembly);

app.MapGraphQL("/graphql"); //GraphQL endpoint before conventional routes (give it priority)
//Use a more specific pattern for the Auth controller to avoid conflicts with Blazor routes
app.MapControllerRoute(name: "auth", pattern: "api/auth/{action=Login}/{id?}", defaults: new { controller = "Auth" });
app.MapControllerRoute(name: "default", pattern: "api/{controller=Home}/{action=Index}/{id?}"); //Other conventional controller routes

app.Run();