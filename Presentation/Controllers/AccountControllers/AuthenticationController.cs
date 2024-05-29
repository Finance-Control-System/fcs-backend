using Domain.CQRS.Requests.Authentication;
using Domain.CQRS.Responses.Authentication;
using Domain.Messages.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Infrastructure;

namespace Presentation.Controllers.AccountControllers;

[AllowAnonymous]
public class AuthenticationController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost("register")]
    public async Task<AuthenticateUserResponse> Register([FromBody] RegisterUserMessage message, CancellationToken cancellationToken) =>
        await Mediator.Send(new RegisterUserRequest(message.Email, message.Password), cancellationToken);

    [HttpPost("login")]
    public async Task<AuthenticateUserResponse> Login([FromBody] SignInUserMessage message, CancellationToken cancellationToken) =>
        await Mediator.Send(new AuthenticateUserRequest(message.Email, message.Password), cancellationToken);

    [Authorize]
    [HttpPost("renewToken")]
    public async Task<AuthenticateUserResponse> RenewToken([FromBody] RenewTokenMessage message, CancellationToken cancellationToken) =>
        await Mediator.Send(new RenewTokenRequest(Request.Headers.Authorization.ToString(), message.RefreshToken), cancellationToken);
}
