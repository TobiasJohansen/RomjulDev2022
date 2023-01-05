namespace RomjulDev2022.Tokens;
public interface ITokenRepository
{
    Task<string?> GetSpotifyRefreshTokenOrDefaultAsync(string userId);

    Task UpsertSpotifyRefreshTokenAsync(string userId, string refreshToken);
}
