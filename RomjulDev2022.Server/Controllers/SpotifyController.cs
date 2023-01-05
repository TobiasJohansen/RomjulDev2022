using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using RomjulDev2022.Tokens;
using RomjulDev2022.Shared;
using System.ComponentModel.DataAnnotations;
using RomjulDev2022.Server.Options;
using Microsoft.Extensions.Options;

namespace RomjulDev2022.Server.Controllers;
[ApiController]
[Route("[controller]")]
public class SpotifyController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;

    public SpotifyController(ITokenService tokenService, HttpClient httpClient, IOptions<SpotifyOptions> options)
    {
        _tokenService = tokenService;
        _httpClient = httpClient;
        _clientId = options.Value.ClientId;
        _clientSecret = options.Value.ClientSecret;
        _redirectUri = options.Value.RedirectUri;
    }

    [HttpGet("login-uri")]
    public IActionResult GetLoginUri()
    {
        var parameters = new string[]
        {
            $"client_id={_clientId}",
            "response_type=code",
            $"redirect_uri={_redirectUri}",
            "scope=user-read-private user-read-email user-top-read user-modify-playback-state"
        };

        return Ok($"https://accounts.spotify.com/authorize?{string.Join("&", parameters)}");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]SpotifyLoginRequest request)
    {
        var oAuthToken = await GetOAuthToken(
            new List<KeyValuePair<string, string>>
            {
                new ("grant_type", "authorization_code"),
                new ("code", request.AuthorizationCode),
                new ("redirect_uri", _redirectUri),
            });

        if (oAuthToken.RefreshToken is not null)
            await _tokenService.UpsertSpotifyRefreshTokenAsync(GetUserId(), oAuthToken.RefreshToken);

        return Ok(oAuthToken);
    }

    [HttpGet("access-token")]
    public async Task<IActionResult> GetSpotifyTokenOrDefault()
    {
        var refreshToken = await _tokenService.GetSpotifyRefreshTokenOrDefaultAsync(GetUserId());

        if (refreshToken is null)
            return NotFound();

        return Ok(await GetOAuthToken(
            new List<KeyValuePair<string, string>>
            {
                new ("grant_type", "refresh_token"),
                new ("refresh_token", refreshToken),
            }));
    }

    private async Task<OAuthToken> GetOAuthToken(List<KeyValuePair<string, string>> content)
    {
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://accounts.spotify.com/api/token"),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}")))
            },
            Content = new FormUrlEncodedContent(content)
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OAuthToken>() ?? new OAuthToken();
    }

    private string GetUserId() => HttpContext.User.Claims
        .Where(c => c.Issuer == "Google")
        .Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
        .Select(c => c.Value)
        .Single();
}
