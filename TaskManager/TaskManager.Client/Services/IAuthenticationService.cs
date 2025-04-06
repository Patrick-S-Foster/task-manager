namespace TaskManager.Client.Services;

public interface IAuthenticationService
{
    event EventHandler? AuthenticationStateChanged;

    bool IsAuthenticated { get; }

    Task<TryResultWhenFalse<(IEnumerable<string> EmailErrors, IEnumerable<string> PasswordErrors)>> TryRegisterAsync(
        string email, string password);

    Task<bool> TryAuthenticateAsync(string email, string password);

    Task<bool> TryAddAuthorizationHeaderAsync(HttpRequestMessage request);

    Task LogoutAsync();
}