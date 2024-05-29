using Domain.CQRS.Requests.Authentication;
using FluentValidation;

namespace Application.Validators.AuthValidators;

public class AuthenticateUserValidator : AbstractValidator<AuthenticateUserRequest>
{
    public AuthenticateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(50);
    }
}
