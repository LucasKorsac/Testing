using MongoDB.Bson;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing.Data
{
    /// <summary>
    /// Генерация синтетических данных
    /// </summary>
    public static class SinteticData
    {
        private static readonly Random _rnd = new();

        public static async Task Init(
            IMongoRepo<Roles> roleRepo,
            IMongoRepo<Developers> developerRepo,
            IMongoRepo<DevelopRoleApplic> devRoleAppRepo,
            IMongoRepo<Applications> appRepo,
            IMongoRepo<MetricTypes> metricTypeRepo,
            IMongoRepo<Metrics> metricRepo,
            IMongoRepo<Instances> instanceRepo,
            IMongoRepo<EquipParam> equipParamRepo,
            IMongoRepo<Values> valueRepo,
            IMongoRepo<ABTests> abTestRepo,
            IMongoRepo<Variants> variantRepo,
            IMongoRepo<AbResults> resultRepo)
        {
            Console.WriteLine("Seeding database...");

            // защита от повторного заполнения
            if (await appRepo.Count() > 0)
            {
                Console.WriteLine("Database already seeded");
                return;
            }

            // Роли

            var roles = new List<Roles>
            {
                new() { Name = "Backend" },
                new() { Name = "Frontend" },
                new() { Name = "QA" },
                new() { Name = "DevOps" },
                new() { Name = "Analyst" },
                new() { Name = "Manager" }
            };

            await roleRepo.CreateMany(roles);

            // РазработчикиDEVELOPERS

            var developers = Enumerable.Range(1, 10)
                .Select(i => new Developers
                {
                    Login = $"developer{i}",
                    PasswordHash = Guid.NewGuid().ToString()
                })
                .ToList();

            await developerRepo.CreateMany(developers);

            // Приложения

            var apps = new List<Applications>
            {
                new()
                {
                    Name = "Shop Platform",
                    Description = "E-commerce system"
                },

                new()
                {
                    Name = "Analytics Dashboard",
                    Description = "Monitoring dashboard"
                },

                new()
                {
                    Name = "Streaming Service",
                    Description = "Video streaming platform"
                },

                new()
                {
                    Name = "Mobile Banking",
                    Description = "Bank application"
                },

                new()
                {
                    Name = "Learning Platform",
                    Description = "Education platform"
                }
            };

            await appRepo.CreateMany(apps);

            // Слабая сущность DRA

            var devRoleApps = new List<DevelopRoleApplic>();

            foreach (var dev in developers)
            {
                devRoleApps.Add(new DevelopRoleApplic
                {
                    DeveloperId = dev.Id,
                    RoleId = roles[_rnd.Next(roles.Count)].Id,
                    Application = apps[_rnd.Next(apps.Count)].Id
                });
            }

            await devRoleAppRepo.CreateMany(devRoleApps);

            // Тип метрики

            var metricTypes = new List<MetricTypes>
            {
                new() { Name = "CTR" },
                new() { Name = "CR" },
                new() { Name = "Retention" },
                new() { Name = "Performance" },
                new() { Name = "UX" },
                new() { Name = "CPU Usage" },
                new() { Name = "Memory" }
            };

            await metricTypeRepo.CreateMany(metricTypes);

            // Метрики

            var metrics = new List<Metrics>();

            foreach (var app in apps)
            {
                foreach (var metricType in metricTypes)
                {
                    metrics.Add(new Metrics
                    {
                        ApplicationId = app.Id,
                        MetricTypeId = metricType.Id,
                        Meaning = Math.Round(_rnd.NextDouble() * 100, 2)
                    });
                }
            }

            await metricRepo.CreateMany(metrics);

            // Экземпляры

            var instances = new List<Instances>();

            foreach (var app in apps)
            {
                for (int i = 1; i <= 5; i++)
                {
                    instances.Add(new Instances
                    {
                        ApplicationId = app.Id,
                        Version = i,
                        Name = $"{app.Name} v{i}",
                        Date = DateTime.UtcNow.AddDays(-_rnd.Next(1, 100))
                    });
                }
            }

            await instanceRepo.CreateMany(instances);

            // Параметры оборудования

            var equipParams = new List<EquipParam>
            {
                new()  {      Environment = "RAM", Recommendation = "Объем оперативной памяти"  },
                new()  { Environment = "CPU", Recommendation = "Модель процессора" },
                new()  {   Environment = "Screen Resolution", Recommendation = "Разрешение экрана"  },
                new()  {  Environment = "Storage",   Recommendation = "Объем хранилища"  },
                new() {  Environment = "GPU",   Recommendation = "Видеокарта устройства"  },
                new() { Environment = "OS Version", Recommendation = "Версия операционной системы"}
            };

            await equipParamRepo.CreateMany(equipParams);

            // Значения параметров

            var values = new List<Values>();

            var metricValues = new Dictionary<string, List<double>>
            {
                ["RAM"] = new() { 4, 8, 16, 32, 64 },
                ["CPU"] = new()  {  101,   102, 103, 104, 105 },
                ["Screen Resolution"] = new() {720, 1080, 1440, 2160},
                ["Storage"] = new() { 128, 256, 512, 1024},
                ["GPU"] = new() {   201,  202, 203, 204},
                ["OS Version"] = new() { 10,11, 12, 13,14}
            };

            foreach (var instance in instances)
            {
                foreach (var param in equipParams)
                {
                    var possibleValues = metricValues[param.Environment];

                    values.Add(new Values
                    {
                        InstanceId = instance.Id, ParamId = param.Id, MetricValue = possibleValues[ _rnd.Next(possibleValues.Count)]
                    });
                }
            }

            await valueRepo.CreateMany(values);

            // AB Тесты

            var tests = new List<ABTests>
            {
                new()
                {
                    Name = "button_color_test", Description = "Button color optimization", Enabled = true
                },

                new()
                {
                    Name = "checkout_ui_test", Description = "Checkout redesign", Enabled = true
                },

                new()
                {
                    Name = "header_layout_test",
                    Description = "Header navigation experiment",
                    Enabled = false
                },

                new()
                {
                    Name = "pricing_page_test",
                    Description = "Pricing page optimization",
                    Enabled = true
                },

                new()
                {
                    Name = "signup_flow_test",
                    Description = "Registration flow experiment",
                    Enabled = false
                }
            };

            await abTestRepo.CreateMany(tests);

            // Варианты

            var variants = new List<Variants>();

            foreach (var test in tests)
            {
                variants.Add(new Variants
                {
                    AbTestId = test.Id,
                    Name = "Variant A",
                    Description = "Default version",
                    Mean = _rnd.Next(10, 90),
                    Audience = 50
                });

                variants.Add(new Variants
                {
                    AbTestId = test.Id,
                    Name = "Variant B",
                    Description = "Experimental version",
                    Mean = _rnd.Next(10, 90),
                    Audience = 50
                });

                variants.Add(new Variants
                {
                    AbTestId = test.Id,
                    Name = "Variant C",
                    Description = "Alternative design",
                    Mean = _rnd.Next(10, 90),
                    Audience = 25
                });
            }

            await variantRepo.CreateMany(variants);

            // Результаты

            var results = new List<AbResults>();

            foreach (var instance in instances)
            {
                for (int i = 0; i < 5; i++)
                {
                    results.Add(new AbResults
                    {
                        InstanceId = instance.Id,
                        VariantId = variants[_rnd.Next(variants.Count)].Id
                    });
                }
            }

            await resultRepo.CreateMany(results);

            Console.WriteLine("Database seeding completed");
        }
    }
}