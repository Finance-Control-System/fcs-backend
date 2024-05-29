namespace Domain.CQRS.Responses.Authentication;

public class AuthenticateUserResponse(
    string authToken,
    string renewToken,
    string email)
{
    public string AuthToken { get; set; } = authToken;

    public string RenewToken { get; set; } = renewToken;

    public string Email { get; set; } = email;
}
