using Microsoft.Azure.Cosmos;
using RomjulDev2022.Database.Models;
using RomjulDev2022.Tokens;

namespace RomjulDev2022.Database;
public class TokenRepository : ITokenRepository
{
    private readonly CosmosClient _client;

    public TokenRepository(CosmosClient client)
    {
        _client = client;
    }

    public async Task<string?> GetSpotifyRefreshTokenOrDefaultAsync(string userId) =>
        (await GetRefreshTokens(await GetRefreshTokenContainer(), userId, "Spotify"))
            .Select(token => token.RefreshToken)
            .SingleOrDefault();

    public async Task UpsertSpotifyRefreshTokenAsync(string userId, string refreshToken)
    {
        var tokenContainer = await GetRefreshTokenContainer();
        var existingToken = (await GetRefreshTokens(await GetRefreshTokenContainer(), userId, "Spotify")).SingleOrDefault();

        if (existingToken != null)
            await tokenContainer.DeleteItemAsync<RefreshTokenDbModel>(existingToken.Id.ToString(), new PartitionKey(userId));

        await tokenContainer.CreateItemAsync(
            new RefreshTokenDbModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RefreshToken = refreshToken,
                System = "Spotify"
            });
    }

    private async Task<IList<RefreshTokenDbModel>> GetRefreshTokens(Container container, string userId, string system)
    {
        using var feed = container
            .GetItemQueryIterator<RefreshTokenDbModel>(
                new QueryDefinition("SELECT * FROM RefreshTokens RT WHERE RT.userId = @userId AND RT.system = @system")
                    .WithParameter("@userId", userId)
                    .WithParameter("@system", system));

        return await ReadAllAsync(feed);
    }

    private async Task<Container> GetRefreshTokenContainer()
    {
        var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync("RomjulDev2022");
        var containerResponse = await databaseResponse.Database
            .DefineContainer("RefreshTokens", "/UserId")
            .WithUniqueKey()
                .Path("/UserId")
                .Path("/System")
            .Attach()
            .CreateIfNotExistsAsync();

        return containerResponse;
    }

    private static async Task<IList<RefreshTokenDbModel>> ReadAllAsync(FeedIterator<RefreshTokenDbModel> feed)
    {
        var users = new List<RefreshTokenDbModel>();
        while (feed.HasMoreResults)
        {
            var response = await feed.ReadNextAsync();
            foreach (var user in response)
                users.Add(user);
        }

        return users;
    }
}
