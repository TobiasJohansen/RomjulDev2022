using System.Text.Json.Serialization;

namespace RomjulDev2022.Models;

public class SpotifyArtists
{
    [JsonPropertyName("next")]
    public string? NextPage { get; set; }

    [JsonPropertyName("previous")]
    public string? PreviousPage { get; set; }

    [JsonPropertyName("items")]
    public IList<SpotifyArtist> Items { get; set; } = new List<SpotifyArtist>();
}
