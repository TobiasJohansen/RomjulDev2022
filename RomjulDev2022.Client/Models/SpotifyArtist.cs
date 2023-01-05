using System.Text.Json.Serialization;

namespace RomjulDev2022.Models;

public class SpotifyArtist
{
    [JsonPropertyName("external_urls")]
    public Dictionary<string, string> ExternalUrls { get; set; } = new Dictionary<string, string>();

    [JsonPropertyName("genres")]
    public IList<string> Genres { get; set; } = new List<string>();

    [JsonPropertyName("images")]
    public IList<SpotifyImage> Images { get; set; } = new List<SpotifyImage>();

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
