namespace RomjulDev2022.Tokens;

public class TokenService : ITokenService
{
    private readonly ITokenRepository _repository;

    public TokenService(ITokenRepository repository)
    {
        _repository = repository;
    }

    public Task<string?> GetSpotifyRefreshTokenOrDefaultAsync(string userId) => 
        _repository.GetSpotifyRefreshTokenOrDefaultAsync(userId);

    public Task UpsertSpotifyRefreshTokenAsync(string userId, string token) =>
        _repository.UpsertSpotifyRefreshTokenAsync(userId, token);
        
}
