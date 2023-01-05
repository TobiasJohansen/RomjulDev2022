namespace RomjulDev2022.Tokens;
public interface ITokenService
{
    public Task<string?> GetSpotifyRefreshTokenOrDefaultAsync(string userId);

    public Task UpsertSpotifyRefreshTokenAsync(string userId, string token);
}
