using Microsoft.Extensions.DependencyInjection;
using AppWeb.ViewModels.Features.Users;

namespace AppWeb.ViewModels;

public static class DependencyInjection
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        // Registrar ViewModels de Usuarios
        services.AddTransient<ICreateUserVM, CreateUserVM>();
        services.AddTransient<IUpdateUserVM, UpdateUserVM>();
        
        // Registrar otros ViewModels cuando estén disponibles
        //services.AddTransient<ITableUserVM, TableUserVM>();
        //services.AddTransient<ILoginViewModel, LoginViewModel>();

        return services;
    }
}