namespace TaskManager.Client.Services;

public interface IAuthenticationService
{
    bool IsAuthenticated { get; }

    Task<TryResultWhenFalse<IEnumerable<string>>> TryRegisterAsync(string email, string password);

    Task<bool> TryAuthenticateAsync(string email, string password);

    Task<bool> TryAddAuthorizationHeaderAsync(HttpRequestMessage request);

    Task LogoutAsync();
}