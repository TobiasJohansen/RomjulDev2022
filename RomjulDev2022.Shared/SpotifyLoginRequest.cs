namespace RomjulDev2022.Shared;

public class SpotifyLoginRequest
{
    public SpotifyLoginRequest(string authorizationCode)
    {
        AuthorizationCode = authorizationCode;
    }

    public string AuthorizationCode { get; }
}
