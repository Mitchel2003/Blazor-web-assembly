using AppWeb.Shared.Inputs;
using FluentValidation;

namespace AppWeb.Shared.Validators;

/// <summary>
/// Synchronous rules that can be evaluated both on the WASM client and on the Server.
/// Anything that requires I/O (e.g., DB checks) must live in a server-side validator.
/// </summary>
public sealed class LoginInputValidator : AbstractValidator<LoginInput>
{
    public LoginInputValidator()
    {
        RuleFor(i => i.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid");

        RuleFor(i => i.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
    }
}