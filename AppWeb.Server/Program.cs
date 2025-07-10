using AppWeb.SharedClient.Services.Graphql;
using AppWeb.Infrastructure;
using AppWeb.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>() //Enables user-secrets
    .AddEnvironmentVariables();

// Keep MVC controllers for API endpoints
// This maintains compatibility with cookie-based authentication
// and allows traditional REST API endpoints alongside GraphQL
builder.Services.AddControllersWithViews();

// Configure HTTP clients for server-side API access
// These are used when server-side components need to access APIs
var apiBase = new Uri(builder.Configuration["ApiBase"] ?? "https://localhost:5001/");
builder.Services.AddHttpClient<IUsersApiClient, UsersApiClient>(c => c.BaseAddress = apiBase);

// Register core application services
// These provide business logic functionality
builder.Services.AddApplication();

// Register infrastructure services
// These provide data access and external service integrations
builder.Services.AddInfrastructure(builder.Configuration);

//Configure CORS to allow the client application to access the API
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration["AllowedOrigins"]?.Split(',') ?? new[] { "https://localhost:5002" })
              .AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

var app = builder.Build();

//Configure the HTTP request pipeline based on environment
if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); } //Enable developer exception page
else { app.UseExceptionHandler("/error"); app.UseHsts(); } //Enable HTTPS Strict Transport Security

//Standard middleware configuration
app.UseHttpsRedirection();
app.UseStaticFiles(); //Serve static content from wwwroot first
app.UseRouting(); //Configure routing middleware

//Enable CORS before authentication middleware
app.UseCors();

//Authentication middleware
app.UseAuthentication();
app.UseAuthorization();

//Map GraphQL endpoint
//This provides the primary API for client-side data access
app.MapGraphQL("/graphql");

//Map conventional controller routes
//Use specific patterns to avoid conflicts with Blazor routes
app.MapControllerRoute(name: "auth", pattern: "api/auth/{action=Login}/{id?}", defaults: new { controller = "Auth" });
app.MapControllerRoute(name: "default", pattern: "api/{controller=Home}/{action=Index}/{id?}"); //Other API endpoints

app.Run();