using AppWeb.Client.Services;
using AppWeb.Infrastructure;
using AppWeb.Application;
using AppWeb.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents(); //Add support for Blazor WebAssembly components
builder.Services.AddControllersWithViews(); //Controllers & Views (optional but keeps compatibility with cookie-auth views)

var apiBase = new Uri(builder.Configuration["ApiBase"] ?? "https://localhost:5001/"); //HTTP clients for prerendered components
builder.Services.AddHttpClient<IUsersApiClient, UsersApiClient>(c => c.BaseAddress = apiBase); //suggested system more reusable

// Clean Architecture layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) { app.UseWebAssemblyDebugging(); }
else { app.UseExceptionHandler("/Error", createScopeForErrors: true); app.UseHsts(); }

app.UseHttpsRedirection();

app.UseStaticFiles(); //Static content for MVC & Blazor
app.UseRouting(); //Routing middleware

// Authentication / Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery(); //Antiforgery middleware
app.MapStaticAssets(); //Static assets middleware

// Razor Components (WASM+SSR)
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(AppWeb.Client._Imports).Assembly);

//GraphQL endpoint and conventional route mapping moved to extension
app.MapAppWebEndpoints();
app.Run();