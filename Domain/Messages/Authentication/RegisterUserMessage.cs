namespace Domain.Messages.Authentication;

public class RegisterUserMessage
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
