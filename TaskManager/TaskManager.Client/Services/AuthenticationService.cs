using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TaskManager.Client.Services;

public class AuthenticationService(IHttpClientFactory httpClientFactory, IEndpointService endpointService)
    : IAuthenticationService
{
    private const string MediaTypeJson = "application/json";

    private static readonly TimeSpan RefreshBeforeExpiration = TimeSpan.FromMinutes(5);

    private string _accessToken = string.Empty;
    private string _refreshToken = string.Empty;
    private DateTime _accessTokenExpiryTime = DateTime.MinValue;

    public bool IsAuthenticated { get; private set; }

    public async Task<TryResultWhenFalse<IEnumerable<string>>> TryRegisterAsync(string email, string password)
    {
        using var httpClient = httpClientFactory.CreateClient();
        using var content = new StringContent(
            JsonSerializer.Serialize(new { email, password }),
            Encoding.UTF8,
            MediaTypeJson);

        var response = await httpClient.PostAsync(endpointService.Register, content);

        if (response.IsSuccessStatusCode)
        {
            return new TryResultWhenFalse<IEnumerable<string>>(true);
        }

        try
        {
            if (await JsonNode.ParseAsync(await response.Content.ReadAsStreamAsync()) is JsonObject body &&
                body["errors"] is JsonObject errors)
            {
                return new TryResultWhenFalse<IEnumerable<string>>(false)
                {
                    Result = errors
                        .Where(pair => pair.Value is JsonArray)
                        .Select(pair => pair.Value)
                        .Cast<JsonArray>()
                        .SelectMany(array => array)
                        .Where(item => item is JsonValue)
                        .Cast<JsonValue>()
                        .Select(item => item.ToString())
                };
            }
        }
        catch (JsonException)
        {
            // This is suppressed to not propagate an exception when the response content is not JSON (e.g., empty)
        }

        return new TryResultWhenFalse<IEnumerable<string>>(false)
        {
            Result = [$"An unexpected response schema was received from the server ({response.StatusCode})."]
        };
    }

    private async Task<bool> TrySetTokens(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode &&
            await JsonNode.ParseAsync(await response.Content.ReadAsStreamAsync()) is JsonObject body &&
            body["accessToken"] is JsonValue accessToken &&
            body["refreshToken"] is JsonValue refreshToken &&
            body["expiresIn"] is JsonValue expiresIn &&
            int.TryParse(expiresIn.ToString(), out var expiresInSeconds))
        {
            _accessToken = accessToken.ToString();
            _refreshToken = refreshToken.ToString();
            _accessTokenExpiryTime = DateTime.Now.AddSeconds(expiresInSeconds);
            return true;
        }

        _accessToken = string.Empty;
        _refreshToken = string.Empty;
        _accessTokenExpiryTime = DateTime.MinValue;
        return false;
    }

    public async Task<bool> TryAuthenticateAsync(string email, string password)
    {
        await LogoutAsync();

        using var httpClient = httpClientFactory.CreateClient();
        using var content = new StringContent(
            JsonSerializer.Serialize(new { email, password }),
            Encoding.UTF8,
            MediaTypeJson);

        IsAuthenticated = await TrySetTokens(await httpClient.PostAsync(endpointService.Login, content));
        return IsAuthenticated;
    }

    private async Task<bool> TryRefreshTokenAsync()
    {
        using var httpClient = httpClientFactory.CreateClient();
        using var content = new StringContent(
            JsonSerializer.Serialize(new { refreshToken = _refreshToken }),
            Encoding.UTF8,
            MediaTypeJson);

        return await TrySetTokens(await httpClient.PostAsync(endpointService.Refresh, content));
    }

    public async Task<bool> TryAddAuthorizationHeaderAsync(HttpRequestMessage request)
    {
        if (!IsAuthenticated ||
            _accessTokenExpiryTime - DateTime.Now <= RefreshBeforeExpiration && !await TryRefreshTokenAsync())
        {
            return false;
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        return true;
    }

    public async Task LogoutAsync()
    {
        using var httpClient = httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, endpointService.Logout);

        if (!await TryAddAuthorizationHeaderAsync(request))
        {
            return;
        }

        IsAuthenticated = false;
        _accessToken = string.Empty;
        _refreshToken = string.Empty;
        _accessTokenExpiryTime = DateTime.MinValue;
        await httpClient.SendAsync(request);
    }
}