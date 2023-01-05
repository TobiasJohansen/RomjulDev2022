using System.Text.Json.Serialization;

namespace RomjulDev2022.Models;

public class SpotifyAlbum
{
    [JsonPropertyName("images")]
    public IList<SpotifyImage> Images { get; set; } = new List<SpotifyImage>();

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
