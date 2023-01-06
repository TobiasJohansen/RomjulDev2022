using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos;
using Azure.Identity;

namespace RomjulDev2022.Server;

internal static class ServiceCollectionExtensions
{
    public static void AddCosmosDb(this IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddSingleton(serviceProvider =>
            new CosmosClientBuilder(
                    config["CosmosDb:AccountEndpoint"]
                        ?? throw new ArgumentException("Missing configuration [CosmosDb:AccountEndpoint]."),
                    config["CosmosDb:AuthKey"]
                        ?? throw new ArgumentException("Missing configuration [CosmosDb:AuthKey]."))
                .WithSerializerOptions(new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
                .Build());
    }
}
