using AppWeb.Application.Handlers;
using AppWeb.Shared.Dtos;
using FluentValidation;

namespace AppWeb.Application.Validators;

// -----------------------------------------------------------------------------
// Aggregate-level base validator (rules common to User DTO)
// -----------------------------------------------------------------------------
public sealed class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(u => u.Email).NotEmpty().WithMessage("Email is required").EmailAddress();
        RuleFor(u => u.Username).NotEmpty().MaximumLength(100);

        // Password rule only if provided (Update scenarios), concrete commands add stricter requirements
        RuleFor(u => u.Password).MinimumLength(6).When(u => !string.IsNullOrEmpty(u.Password));
    }
}

// -----------------------------------------------------------------------------
// Command-specific validator: CREATE
// -----------------------------------------------------------------------------
public sealed class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(c => c.Input).NotNull();
        RuleFor(c => c.Input!).SetValidator(new UserValidator())
            .DependentRules(() => RuleFor(c => c.Input!.Password).NotEmpty().MinimumLength(6));
    }
}

// -----------------------------------------------------------------------------
// Command-specific validator: UPDATE
// -----------------------------------------------------------------------------
public sealed class UpdateUserValidator : AbstractValidator<UpdateUser>
{
    public UpdateUserValidator()
    {
        RuleFor(c => c.Input).NotNull();
        RuleFor(c => c.Input!).SetValidator(new UserValidator());
    }
}