using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing.Data
{
    public static class SinteticData
    {
        private static readonly Random _rnd = new();

       /// <summary>
       /// Инициализация тестовых данных
       /// </summary>
        public static async Task Init(IMongoRepo<Companies> companyRepo, IMongoRepo<Roles> roleRepo,
            IMongoRepo<Developers> developerRepo, IMongoRepo<Applications> appRepo, IMongoRepo<MetricTypes> metricTypeRepo,
            IMongoRepo<Metrics> metricRepo, IMongoRepo<Instances> instanceRepo, IMongoRepo<Attributes> attributeRepo,
            IMongoRepo<Values> valueRepo, IMongoRepo<ABDescriptions> descRepo, IMongoRepo<ABTests> abTestRepo,
            IMongoRepo<Variants> variantRepo,  IMongoRepo<AbResults> resultRepo)
        {
            Console.WriteLine("Seeding database...");

            // Защита от повторного заполнения
            if (await companyRepo.Count() > 0)
            {
                Console.WriteLine("Database already seeded");
                return;
            }

            var company = new Companies { Name = "Tech Corp" };
            await companyRepo.Create(company);

            var roles = new List<Roles>
        {
            new() { Name = "Backend" }, new() { Name = "Frontend" }, new() { Name = "QA" } };
            await roleRepo.CreateMany(roles);

            var developers = Enumerable.Range(1, 5).Select(i => new Developers
            { CompanyId = company.Id, RoleId = roles[_rnd.Next(roles.Count)].Id, Login = $"user{i}", Password = "1234", PasswordHash = Guid.NewGuid().ToString()}).ToList();

            await developerRepo.CreateMany(developers);

            var app = new Applications
            {
                CompanyId = company.Id, Name = "Test App", Description = "Demo application"
            };

            await appRepo.Create(app);

            var metricTypes = new List<MetricTypes>
            { new() { Name = "Performance" }, new() { Name = "UX" } };
            await metricTypeRepo.CreateMany(metricTypes);

            var metrics = new List<Metrics>
            { new() { Name = "Load Time", MetricTypeId = metricTypes[0].Id }, new() { Name = "Click Rate", MetricTypeId = metricTypes[1].Id }};
            await metricRepo.CreateMany(metrics);

            var instances = metrics.Select((m, i) => new Instances
            {
                ApplicationId = app.Id, MetricId = m.Id, Version = i + 1, Name = $"Instance v{i + 1}"}).ToList();

            await instanceRepo.CreateMany(instances);

            var attributes = new List<Attributes>
            {
                new() { Environment = "Prod", Recommendation = "Stable" }, new() { Environment = "Test", Recommendation = "Check performance" }
            };

            await attributeRepo.CreateMany(attributes);

            var values = new List<Values>();

            foreach (var inst in instances)
            {
                foreach (var attr in attributes)
                {
                    values.Add(new Values
                    {
                        InstanceId = inst.Id, AttributeId = attr.Id,
                        Date = DateTime.UtcNow.AddDays(-_rnd.Next(10)), MetricValue = _rnd.NextDouble() * 100
                    });
                }
            }

            await valueRepo.CreateMany(values);

            var desc = new ABDescriptions
            {
                Target = "Increase CTR", Description = "Button test"
            };

            await descRepo.Create(desc);

            var abTest = new ABTests
            {
                DescriptionId = desc.Id,
                Name = "Button Color Test"
            };

            await abTestRepo.Create(abTest);

            var variants = new List<Variants>
            {
                new() { AbTestId = abTest.Id, Name = "Red", Description = "Red button" },
                new() { AbTestId = abTest.Id, Name = "Blue", Description = "Blue button" },
                new() { AbTestId = abTest.Id, Name = "Green", Description = "Green button" }
            };

            await variantRepo.CreateMany(variants);

            var results = new List<AbResults>();

            foreach (var inst in instances)
            {
                results.Add(new AbResults
                {
                    InstanceId = inst.Id, VariantId = variants[_rnd.Next(variants.Count)].Id
                });
            }

            await resultRepo.CreateMany(results);

            Console.WriteLine("Seeding completed");
        }
    }
}