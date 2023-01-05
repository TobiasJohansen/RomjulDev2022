using System.Text.Json.Serialization;

namespace RomjulDev2022.Shared;

public class OAuthToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("scope")]
    public string? Scope { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.Now;

    public bool HasExpired() => DateTime.Now > Created.AddSeconds(ExpiresIn - 5);
}
