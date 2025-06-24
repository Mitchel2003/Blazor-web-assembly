using AppWeb.Application.Graphql.Handlers;
using AppWeb.Domain.Interfaces;
using AppWeb.Shared.Dtos;
using FluentValidation;

namespace AppWeb.Infrastructure.Persistence.Validators;

// -----------------------------------------------------------------------------
// Aggregate-level base validator (rules common to User DTO)
// -----------------------------------------------------------------------------
public sealed class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(u => u.Username).NotEmpty().MaximumLength(20);
        RuleFor(u => u.Email).NotEmpty().WithMessage("Email is required").EmailAddress();
        RuleFor(u => u.Password).MinimumLength(6).When(u => !string.IsNullOrEmpty(u.Password));
    }
}

// -----------------------------------------------------------------------------
// Command-specific validator: CREATE
// -----------------------------------------------------------------------------
public sealed class CreateUserValidator : AbstractValidator<CreateUser>
{
    private readonly IUserRepository _repo;
    public CreateUserValidator(IUserRepository repo)
    {
        _repo = repo;
        RuleFor(c => c.Input).NotNull();
        RuleFor(c => c.Input!).SetValidator(new UserValidator());
        RuleFor(c => c.Input!.Email).MustAsync(EmailUnique).WithMessage("Email ya existe.");
    }

    private async Task<bool> EmailUnique(string email, CancellationToken ct)
    { var existing = await _repo.GetByFilterAsync(u => u.Email == email); return !existing.Any(); }
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