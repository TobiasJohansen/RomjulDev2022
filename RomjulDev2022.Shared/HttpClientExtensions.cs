using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RomjulDev2022.Shared;

public static class HttpClientExtensions
{
    public static async Task SendAndValidateAsync(this HttpClient httpClient, HttpRequestMessage request, string? accessToken = default)
    {
        await SendAndValidate(httpClient, request, accessToken);
    }

    public static async Task<T?> SendAndValidateAsync<T>(this HttpClient httpClient, HttpRequestMessage request, string? accessToken = default)
    {
        var response = await SendAndValidate(httpClient, request, accessToken);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    private static async Task<HttpResponseMessage> SendAndValidate(this HttpClient httpClient, HttpRequestMessage request, string? accessToken = default)
    {
        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return response;
    }
}
