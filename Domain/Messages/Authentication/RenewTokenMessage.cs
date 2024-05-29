namespace Domain.Messages.Authentication;

public class RenewTokenMessage
{
    public string RefreshToken { get; set; } = string.Empty;
}
