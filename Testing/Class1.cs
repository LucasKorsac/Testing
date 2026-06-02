// Добавьте в конец файла Facade.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing
{
    public static class TestingRegistration
    {
        public static IServiceCollection AddTesting(this IServiceCollection services, IConfiguration config)
        {
            // MongoDB
            var connectionString = config.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
            var databaseName = config["DatabaseName"] ?? "ABTesting";

            services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
            services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(databaseName);
            });

            // Репозитории
            services.AddScoped(typeof(IMongoRepo<>), typeof(MongoRepo<>));

            // Facade
            services.AddScoped<Facade>(sp =>
            {
                return new Facade(
                    sp.GetRequiredService<IMongoRepo<ABTests>>(),
                    sp.GetRequiredService<IMongoRepo<Variants>>(),
                    sp.GetRequiredService<IMongoRepo<AbResults>>(),
                    sp.GetRequiredService<IMongoRepo<Instances>>(),
                    sp.GetRequiredService<IMongoRepo<Applications>>(),
                    sp.GetRequiredService<IMongoRepo<DevelopRoleApplic>>(),
                    sp.GetRequiredService<IMongoRepo<Metrics>>(),
                    sp.GetRequiredService<IMongoRepo<MetricTypes>>(),
                    sp.GetRequiredService<IMongoRepo<Roles>>(),
                    sp.GetRequiredService<IMongoRepo<Developers>>(),
                    sp.GetRequiredService<IMongoRepo<EquipParam>>(),
                    sp.GetRequiredService<IMongoRepo<Values>>()
                );
            });

            // StatsBuilder
            services.AddScoped<IStatsBuilder, StatsBuilder>();

            return services;
        }
    }
}