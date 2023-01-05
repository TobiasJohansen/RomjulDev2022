using System.Text.Json.Serialization;

namespace RomjulDev2022.Models;

public class SpotifyProfile
{
    [JsonPropertyName("display_name")]
    public string? Name { get; set; }
}
