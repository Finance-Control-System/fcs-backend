using Domain.CQRS.Responses.Authentication;
using MediatR;

namespace Domain.CQRS.Requests.Authentication;

public class RegisterUserRequest(string email, string password) : IRequest<AuthenticateUserResponse>
{
    public string Email { get; set; } = email.ToLower();

    public string Password { get; set; } = password;
}
