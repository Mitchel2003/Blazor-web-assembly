using AppWeb.Shared.Inputs;
using FluentValidation;

namespace AppWeb.Shared.Validators;

/// <summary>
/// Synchronous validator for <see cref="UpdateUserInput"/> that can run on both WASM client
/// and Server. Only checks local rules that do not require I/O.
/// </summary>
public sealed class UpdateUserInputValidator : AbstractValidator<UpdateUserInput>
{
    public UpdateUserInputValidator()
    {
        RuleFor(i => i.Id)
            .GreaterThan(0);

        RuleFor(i => i.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long");

        RuleFor(i => i.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid");

        RuleFor(i => i.Password)
            .Must(p => string.IsNullOrEmpty(p) || p.Length >= 6)
            .WithMessage("Password must be at least 6 characters long when provided");
    }
}