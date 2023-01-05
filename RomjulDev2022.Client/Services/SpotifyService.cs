using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using RomjulDev2022.Models;
using RomjulDev2022.Shared;
using System.Net.Http.Json;

namespace RomjulDev2022.Client.Services;

internal class SpotifyService
{
    private OAuthToken? _oAuthToken;
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private readonly SpotifyClient _spotifyClient;

    public SpotifyService(
        NavigationManager navigationManager,
        HttpClient httpClient,
        IConfiguration config)
    {
        _navigationManager = navigationManager;
        _httpClient = httpClient;
        _spotifyClient = new SpotifyClient(httpClient, new Uri(config["Spotify:BaseUri"]!));
    }

    public async Task<T?> GetAsync<T>(Uri absoluteUri) =>
        await _spotifyClient.GetAsync<T>(absoluteUri, (await GetOAuthTokenAsync()).AccessToken);

    public async Task<SpotifyProfile?> GetSpotifyUserAsync() =>
        await _spotifyClient.GetAsync<SpotifyProfile>(
            new("v1/me", UriKind.Relative), 
            (await GetOAuthTokenAsync()).AccessToken);

    public async Task<T?> GetSpotifyTopListAsync<T>(
        SpotifyTopType topTypeEnum,
        SpotifyTimeInterval timeRangeEnum)
    {
        var topType = topTypeEnum switch
        {
            SpotifyTopType.Artists => "artists",
            SpotifyTopType.Tracks => "tracks",
            _ => throw new Exception($"Unexpected value for {nameof(SpotifyTopType)}, {topTypeEnum}.")
        };

        var timeRange = timeRangeEnum switch
        {
            SpotifyTimeInterval.Short => "short_term",
            SpotifyTimeInterval.Medium => "medium_term",
            SpotifyTimeInterval.Long => "long_term",
            _ => throw new Exception($"Unexpected value for {nameof(SpotifyTimeInterval)}, {timeRangeEnum}.")
        };

        var uri = new Uri($"v1/me/top/{topType}?time_range={timeRange}", UriKind.Relative);
        var accessToken = (await GetOAuthTokenAsync()).AccessToken;

        return await _spotifyClient.GetAsync<T>(
            new($"v1/me/top/{topType}?time_range={timeRange}", UriKind.Relative),
            (await GetOAuthTokenAsync()).AccessToken);
    }

    public async Task PlayTrackAsync(string trackId) =>
        await _spotifyClient.PutAsync(
            new Uri("/v1/me/player/play", UriKind.Relative),
            JsonContent.Create(new SpotifyPlayTrackRequest(trackId)),
             (await GetOAuthTokenAsync()).AccessToken);

    public async Task<OAuthToken?> SignIn()
    {
        _oAuthToken = await GetCachedOAuthTokenAsync();

        if (_oAuthToken?.HasExpired() ?? true)
        {
            var response = await _httpClient.GetAsync(new Uri("spotify/login-uri", UriKind.Relative));
            response.EnsureSuccessStatusCode();
            var test = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(test))
                _navigationManager.NavigateTo(test);
        }

        return _oAuthToken;
    }

    private async Task<OAuthToken> GetOAuthTokenAsync()
    {
        if (_oAuthToken?.HasExpired() ?? true)
            await SignIn();

        return _oAuthToken ?? throw new Exception($"Unable to get {nameof(OAuthToken)} for SpotifyClient.");
    }

    private async Task<OAuthToken?> GetCachedOAuthTokenAsync()
    {
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("code", out var authorizationCode))
        {
            return await _httpClient.SendAndValidateAsync<OAuthToken>(
                new HttpRequestMessage(HttpMethod.Post, new Uri("spotify/login", UriKind.Relative))
                {
                    Content = JsonContent.Create(new SpotifyLoginRequest(authorizationCode!))
                });
        }

        return await GetOAuthTokenFromServerOrDefaultAsync();
    }

    private async Task<OAuthToken?> GetOAuthTokenFromServerOrDefaultAsync()
    {
        var accessTokenResponse = await _httpClient.GetAsync(new Uri("spotify/access-token", UriKind.Relative));

        if (!accessTokenResponse.IsSuccessStatusCode)
            return default;

        return await accessTokenResponse.Content.ReadFromJsonAsync<OAuthToken>();
    }
}
