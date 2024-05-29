using Domain.CQRS.Responses.Authentication;
using MediatR;

namespace Domain.CQRS.Requests.Authentication;

public class RenewTokenRequest(string requestHeader, string renewToken) : IRequest<AuthenticateUserResponse>
{
    public string AuthToken { get; set; } = requestHeader.Replace("Bearer ", "");

    public string RenewToken { get; set; } = renewToken;
}
