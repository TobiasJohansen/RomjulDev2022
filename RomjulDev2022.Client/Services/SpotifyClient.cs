using RomjulDev2022.Shared;

namespace RomjulDev2022.Client.Services;

public class SpotifyClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUri;

    public SpotifyClient(HttpClient httpClient, Uri baseUri)
    {
        _httpClient = httpClient;
        _baseUri = baseUri;
    }

    public Task<T?> GetAsync<T>(Uri uri, string accessToken) =>
        _httpClient.SendAndValidateAsync<T>(
            new(HttpMethod.Get, CombineWithBaseIfRelative(uri)), accessToken);

    public Task PutAsync(Uri uri, HttpContent content, string accessToken) =>
        _httpClient.SendAndValidateAsync(
            new(HttpMethod.Put, CombineWithBaseIfRelative(uri)) { Content = content }, accessToken);

    private Uri CombineWithBaseIfRelative(Uri uri) => uri.IsAbsoluteUri ? uri : new Uri(_baseUri, uri);
}
