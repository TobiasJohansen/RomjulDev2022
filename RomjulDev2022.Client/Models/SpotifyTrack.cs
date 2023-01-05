using System.Text.Json.Serialization;

namespace RomjulDev2022.Models;

public class SpotifyTrack
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("album")]
    public SpotifyAlbum? Album { get; set; }

    [JsonPropertyName("external_urls")]
    public Dictionary<string, string> ExternalUrls { get; set; } = new Dictionary<string, string>();

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
