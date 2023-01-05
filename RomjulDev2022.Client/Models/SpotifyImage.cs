using System.Text.Json.Serialization;

namespace RomjulDev2022.Models;

public class SpotifyImage
{
    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }
}
