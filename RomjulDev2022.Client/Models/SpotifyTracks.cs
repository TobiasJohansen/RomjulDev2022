using System.Text.Json.Serialization;

namespace RomjulDev2022.Models;

public class SpotifyTracks
{
    [JsonPropertyName("next")]
    public string? NextPage { get; set; }

    [JsonPropertyName("items")]
    public IList<SpotifyTrack> Items { get; set; } = new List<SpotifyTrack>();

    [JsonPropertyName("previous")]
    public string? PreviousPage { get; set; }
}
