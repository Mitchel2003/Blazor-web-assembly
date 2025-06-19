namespace AppWeb.Infrastructure;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapAppWebEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGraphQL("/graphql"); //GraphQL endpoint
        endpoints.MapControllerRoute(name: "default",pattern: "{controller=Auth}/{action=Login}/{id?}"); //Conventional MVC route (Auth, etc.)
        return endpoints;
    }
}