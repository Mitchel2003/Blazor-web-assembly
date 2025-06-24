using AppWeb.Server.Models;
using AppWeb.Shared.Dtos;
using Mapster;

namespace AppWeb.Server.Application.Mapping;

/// <summary>
/// Mapster configuration for User aggregate.
/// Uses conventions but explicit registration keeps intentions clear and allows future custom rules.
/// </summary>
public sealed class UserMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserDto, User>(); //DTO -> Entity
        config.NewConfig<User, UserDto>(); //Entity -> DTO
    }
}