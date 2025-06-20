using AppWeb.Shared.Dtos;
using AppWeb.Models;
using Mapster;

namespace AppWeb.Application.Mapping;

/// <summary>
/// Mapster configuration for User aggregate.
/// Uses conventions but explicit registration keeps intentions clear and allows future custom rules.
/// </summary>
public sealed class UserMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserDto, User>(); //DTO -> Entity
        config.NewConfig<User, UserDto>(); //Entity -> DTO
    }
}