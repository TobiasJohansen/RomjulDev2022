using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RomjulDev2022.Database;

public static class ServiceCollectionExtensions
{
    public static void AddCosmosDbClient(this IServiceCollection serviceCollection, IConfiguration config)
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
