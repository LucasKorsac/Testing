using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing
{
    public static class SinteticData
    {
        private static readonly Random _rnd = new();

        public static async Task Init()
        {
            Console.WriteLine("Seeding database...");

            // Защита от повторного заполнения
            if (await Repos.Company.Count() > 0)
            {
                Console.WriteLine("Database already seeded");
                return;
            }

            // Компании
            var company = new Companies { Name = "Tech Corp" };
            await Repos.Company.Create(company);

            // Роли
            var roles = new List<Roles>
            {
                new() { Name = "Backend" },
                new() { Name = "Frontend" },
                new() { Name = "QA" }
            };
            await Repos.Role.CreateMany(roles);

            // Разработчики
            var developers = Enumerable.Range(1, 5).Select(i => new Developers
            {
                CompanyId = company.Id,
                RoleId = roles[_rnd.Next(roles.Count)].Id,
                Login = $"user{i}",
                Password = "1234",
                PasswordHash = Guid.NewGuid().ToString()
            }).ToList();

            await Repos.Developer.CreateMany(developers);

            // Приложение
            var app = new Applications
            {
                CompanyId = company.Id,
                Name = "Test App",
                Description = "Demo application"
            };
            await Repos.Application.Create(app);

            // Типы метрик
            var metricTypes = new List<MetricTypes>
            {
                new() { Name = "Performance" },
                new() { Name = "UX" }
            };
            await Repos.MetricType.CreateMany(metricTypes);

            // Метрики
            var metrics = new List<Metrics>
            {
                new() { Name = "Load Time", MetricTypeId = metricTypes[0].Id },
                new() { Name = "Click Rate", MetricTypeId = metricTypes[1].Id }
            };
            await Repos.Metric.CreateMany(metrics);

            // Инстансы
            var instances = metrics.Select((m, i) => new Instances
            {
                ApplicationId = app.Id,
                MetricId = m.Id,
                Version = i + 1,
                Name = $"Instance v{i + 1}"
            }).ToList();

            await Repos.Instance.CreateMany(instances);

            // Атрибуты
            var attributes = new List<Attributes>
            {
                new() { Environment = "Prod", Recommendation = "Stable" },
                new() { Environment = "Test", Recommendation = "Check performance" }
            };

            await Repos.Attribute.CreateMany(attributes);

            // Значения метрик
            var values = new List<Values>();

            foreach (var inst in instances)
            {
                foreach (var attr in attributes)
                {
                    values.Add(new Values
                    {
                        InstanceId = inst.Id,
                        AttributeId = attr.Id,
                        Date = DateTime.UtcNow.AddDays(-_rnd.Next(10)),
                        MetricValue = _rnd.NextDouble() * 100
                    });
                }
            }

            await Repos.Value.CreateMany(values);

            // A/B описание
            var desc = new ABDescriptions
            {
                Target = "Increase CTR",
                Description = "Button test"
            };

            await Repos.Description.Create(desc);

            // A/B тест
            var abTest = new ABTests
            {
                //ApplicationId = app.Id,
                DescriptionId = desc.Id,
                Name = "Button Color Test"
            };

            await Repos.AbTest.Create(abTest);

            // Варианты
            var variants = new List<Variants>
            {
                new() { AbTestId = abTest.Id, Name = "Red", Description = "Red button" },
                new() { AbTestId = abTest.Id, Name = "Blue", Description = "Blue button" },
                new() { AbTestId = abTest.Id, Name = "Green", Description = "Green button" }
            };

            await Repos.Variant.CreateMany(variants);

            // Результаты
            var results = new List<Results>();

            foreach (var inst in instances)
            {
                results.Add(new Results
                {
                    InstanceId = inst.Id,
                    VariantId = variants[_rnd.Next(variants.Count)].Id
                });
            }

            await Repos.Result.CreateMany(results);

            Console.WriteLine("Seeding completed");
        }
    }
}