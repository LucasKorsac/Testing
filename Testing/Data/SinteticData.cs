using MongoDB.Bson;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing.Data
{
    /// <summary>
    /// Генерация синтетических данных (RU версия)
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
            Console.WriteLine("Заполнение базы тестовыми данными...");

            if (await appRepo.Count() > 0)
            {
                Console.WriteLine("База уже заполнена");
                return;
            }

            //  Роли

            var roles = new List<Roles>
            {
                new() { Name = "Бэкенд-разработчик" },
                new() { Name = "Фронтенд-разработчик" },
                new() { Name = "Тестировщик (QA)" },
                new() { Name = "DevOps инженер" },
                new() { Name = "Аналитик данных" },
                new() { Name = "Менеджер проекта" },
                new() { Name = "Системный архитектор" },
                new() { Name = "Mobile разработчик" },
                new() { Name = "ML инженер" },
                new() { Name = "UX/UI дизайнер" },
                new() { Name = "Support инженер" },
                new() { Name = "Data Engineer" }
            };

            await roleRepo.CreateMany(roles);

            //  Разработчики

            var developers = Enumerable.Range(1, 15)
                .Select(i => new Developers
                {
                    Login = $"разработчик{i}",
                    PasswordHash = Guid.NewGuid().ToString()
                })
                .ToList();

            await developerRepo.CreateMany(developers);

            // Приложения

            var apps = new List<Applications>
            {
                new() { Name = "Интернет-магазин", Description = "Платформа электронной коммерции" },
                new() { Name = "Панель аналитики", Description = "BI система" },
                new() { Name = "Видеосервис", Description = "Стриминг платформа" },
                new() { Name = "Мобильный банк", Description = "Финансовое приложение" },
                new() { Name = "Образовательная платформа", Description = "E-learning система" },
                new() { Name = "CRM система", Description = "Управление клиентами" },
                new() { Name = "HR платформа", Description = "Подбор персонала" },
                new() { Name = "Маркетплейс", Description = "Торговая платформа" },
                new() { Name = "Система логистики", Description = "Доставка и трекинг" },
                new() { Name = "Социальная сеть", Description = "Коммуникационная платформа" }
            };

            await appRepo.CreateMany(apps);

            // Связи разработчиков
            var devRoleApps = developers.Select(dev => new DevelopRoleApplic
            {
                DeveloperId = dev.Id,
                RoleId = roles[_rnd.Next(roles.Count)].Id,
                Application = apps[_rnd.Next(apps.Count)].Id
            }).ToList();

            await devRoleAppRepo.CreateMany(devRoleApps);

            // Типы метрик

            var metricTypes = new List<MetricTypes>
            {
                new() { Name = "CTR" },
                new() { Name = "CR" },
                new() { Name = "Retention" },
                new() { Name = "Latency" },
                new() { Name = "Throughput" },
                new() { Name = "CPU Load" },
                new() { Name = "Memory Usage" },
                new() { Name = "Error Rate" },
                new() { Name = "Session Time" },
                new() { Name = "UX Score" }
            };

            await metricTypeRepo.CreateMany(metricTypes);

            // Метрики

            var metrics = new List<Metrics>();

            foreach (var app in apps)
                foreach (var metricType in metricTypes)
                {
                    metrics.Add(new Metrics
                    {
                        ApplicationId = app.Id,
                        MetricTypeId = metricType.Id,
                        Meaning = Math.Round(_rnd.NextDouble() * 100, 2)
                    });
                }

            await metricRepo.CreateMany(metrics);

            // Экземпляры

            var instances = new List<Instances>();

            foreach (var app in apps)
            {
                for (int i = 1; i <= 10; i++)
                {
                    instances.Add(new Instances
                    {
                        ApplicationId = app.Id,
                        Version = i,
                        Name = $"{app.Name} v{i}",
                        Date = DateTime.UtcNow.AddDays(-_rnd.Next(1, 120))
                    });
                }
            }

            await instanceRepo.CreateMany(instances);

            // Парамерты устройств

            var equipParams = new List<EquipParam>
            {
                new() { Name = "ОЗУ" },
                new() { Name = "CPU" },
                new() { Name = "Screen" },
                new() { Name = "Storage" },
                new() { Name = "GPU" },
                new() { Name = "OS" },
                new() { Name = "Network" },
                new() { Name = "Battery" },
                new() { Name = "Temperature" },
                new() { Name = "Architecture" }
            };

            await equipParamRepo.CreateMany(equipParams);

            // Значения

            var valueMap = new Dictionary<string, List<double>>
            {
                ["ОЗУ"] = new() { 4, 8, 16, 32, 64 },
                ["CPU"] = new() { 100, 200, 300, 400 },
                ["Screen"] = new() { 720, 1080, 1440, 2160 },
                ["Storage"] = new() { 128, 256, 512, 1024 },
                ["GPU"] = new() { 1, 2, 3, 4 },
                ["OS"] = new() { 10, 11, 12, 13 },
                ["Network"] = new() { 100, 200, 500 },
                ["Battery"] = new() { 3000, 4000, 5000 },
                ["Temperature"] = new() { 30, 50, 70 },
                ["Architecture"] = new() { 64, 128 }
            };

            var values = new List<Values>();

            foreach (var instance in instances)
                foreach (var param in equipParams)
                {
                    var list = valueMap[param.Name];

                    values.Add(new Values
                    {
                        InstanceId = instance.Id,
                        ParamId = param.Id,
                        MetricValue = list[_rnd.Next(list.Count)]
                    });
                }

            await valueRepo.CreateMany(values);

            // A/B тесты

            var tests = new List<ABTests>
            {
                new() { Name = "цвет_кнопки", Description = "CTA тест", Enabled = true },
                new() { Name = "чекаут", Description = "Оптимизация оплаты", Enabled = true },
                new() { Name = "шапка", Description = "Навигация", Enabled = false },
                new() { Name = "тарифы", Description = "Pricing page", Enabled = true },
                new() { Name = "регистрация", Description = "Signup flow", Enabled = false }
            };

            await abTestRepo.CreateMany(tests);

            //  Варианты

            var variants = new List<Variants>();

            foreach (var test in tests)
            {
                variants.AddRange(new[]
                {
                    new Variants
                    {
                        AbTestId = test.Id,
                        Name = "A",
                        Description = "Control",
                        Mean = _rnd.Next(20, 80),
                        Audience = 40
                    },
                    new Variants
                    {
                        AbTestId = test.Id,
                        Name = "B",
                        Description = "Variant",
                        Mean = _rnd.Next(20, 80),
                        Audience = 40
                    },
                    new Variants
                    {
                        AbTestId = test.Id,
                        Name = "C",
                        Description = "Alternative",
                        Mean = _rnd.Next(20, 80),
                        Audience = 20
                    }
                });
            }

            await variantRepo.CreateMany(variants);

            //Результаты

            var results = new List<AbResults>();

            foreach (var instance in instances)
            {
                for (int i = 0; i < 10; i++)
                {
                    results.Add(new AbResults
                    {
                        InstanceId = instance.Id,
                        VariantId = variants[_rnd.Next(variants.Count)].Id
                    });
                }
            }

            await resultRepo.CreateMany(results);

            Console.WriteLine("Заполнение базы завершено");
        }
    }
}