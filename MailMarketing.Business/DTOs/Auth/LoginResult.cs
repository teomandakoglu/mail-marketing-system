namespace MailMarketing.Business.DTOs.Auth;

public enum LoginFailureReason
{
    UserNotFound,
    WrongPassword
}

public sealed class LoginResult
{
    public string? Token { get; init; }

    public LoginFailureReason? FailureReason { get; init; }
}
