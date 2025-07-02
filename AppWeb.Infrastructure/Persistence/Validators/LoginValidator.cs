using AppWeb.Application.Graphql.Handlers;
using AppWeb.Domain.Interfaces;
using FluentValidation;

namespace AppWeb.Infrastructure.Persistence.Validators;

// -----------------------------------------------------------------------------
// Command-specific validator: LOGIN
// -----------------------------------------------------------------------------
public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Input.Email).EmailAddress().NotEmpty();
        RuleFor(x => x.Input.Password).NotEmpty();
    }
}