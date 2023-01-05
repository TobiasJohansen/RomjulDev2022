using System.Text.Json.Serialization;

namespace RomjulDev2022.Models;

public class SpotifyPlayTrackRequest
{
    public SpotifyPlayTrackRequest(string trackId)
    {
        Uris = new List<string> { $"spotify:track:{trackId}" };
    }

    [JsonPropertyName("uris")]
    public IList<string> Uris { get; }
}
