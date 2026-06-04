using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Base;
using static Testing.Base.BaseMongo;
using MongoDB.Bson;

namespace ABProjectTests.BaseTest
{
    public class SinteticData
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

            // Количество записей
            const int appsCount = 50;
            const int developersCount = 100;
            const int rolesCount = 15;
            const int metricTypesCount = 12;
            const int equipParamsCount = 20;
            const int testsCount = 200;
            const int variantsPerTest = 4;
            const int instancesPerApp = 15;
            const int resultsPerInstance = 50;

            if (await appRepo.Count() > 0)
            {
                Console.WriteLine("База уже заполнена, очищаем...");
                await ClearAll(roleRepo, developerRepo, devRoleAppRepo, appRepo,
                    metricTypeRepo, metricRepo, instanceRepo, equipParamRepo,
                    valueRepo, abTestRepo, variantRepo, resultRepo);
            }

            Console.WriteLine($"Генерация {appsCount} приложений...");
            // Приложения
            var apps = new List<Applications>();
            for (int i = 1; i <= appsCount; i++)
            {
                apps.Add(new Applications
                {
                    Name = $"Приложение_{i}",
                    Description = $"Описание приложения {i}"
                });
            }
            await appRepo.CreateMany(apps);
            Console.WriteLine($"✓ Создано {apps.Count} приложений");

            Console.WriteLine($"Генерация {rolesCount} ролей...");
            // Роли
            var roles = new List<Roles>();
            for (int i = 1; i <= rolesCount; i++)
            {
                roles.Add(new Roles { Name = $"Роль_{i}" });
            }
            await roleRepo.CreateMany(roles);
            Console.WriteLine($"✓ Создано {roles.Count} ролей");

            Console.WriteLine($"Генерация {developersCount} разработчиков...");
            // Разработчики
            var developers = new List<Developers>();
            for (int i = 1; i <= developersCount; i++)
            {
                developers.Add(new Developers
                {
                    Login = $"dev_{i}",
                    PasswordHash = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                });
            }
            await developerRepo.CreateMany(developers);
            Console.WriteLine($"✓ Создано {developers.Count} разработчиков");

            Console.WriteLine($"Генерация связей разработчиков с приложениями...");
            // Связи разработчиков с приложениями
            var devRoleApps = new List<DevelopRoleApplic>();
            foreach (var dev in developers)
            {
                var assignedApps = apps.OrderBy(_ => _rnd.Next()).Take(_rnd.Next(1, 5));
                foreach (var app in assignedApps)
                {
                    devRoleApps.Add(new DevelopRoleApplic
                    {
                        DeveloperId = dev.Id,
                        RoleId = roles[_rnd.Next(roles.Count)].Id,
                        Application = app.Id
                    });
                }
            }
            await devRoleAppRepo.CreateMany(devRoleApps);
            Console.WriteLine($"✓ Создано {devRoleApps.Count} связей");

            Console.WriteLine($"Генерация {metricTypesCount} типов метрик...");
            // Типы метрик
            var metricTypes = new List<MetricTypes>
            {
                new() { Name = "Удержание (Retention)" },
                new() { Name = "Конверсия (Conversion)" },
                new() { Name = "CTR (Click-Through Rate)" },
                new() { Name = "Среднее время сессии" },
                new() { Name = "Частота ошибок" },
                new() { Name = "Латентность (Latency)" },
                new() { Name = "DAU (Daily Active Users)" },
                new() { Name = "MAU (Monthly Active Users)" },
                new() { Name = "ARPU (Average Revenue Per User)" },
                new() { Name = "Отток (Churn Rate)" },
                new() { Name = "Время загрузки" },
                new() { Name = "Успешность запросов" }
            };
            await metricTypeRepo.CreateMany(metricTypes);
            Console.WriteLine($"✓ Создано {metricTypes.Count} типов метрик");

            Console.WriteLine($"Генерация метрик для приложений...");
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
                        Meaning = Math.Round(10 + _rnd.NextDouble() * 90, 2)
                    });
                }
            }
            await metricRepo.CreateMany(metrics);
            Console.WriteLine($"✓ Создано {metrics.Count} метрик");

            Console.WriteLine($"Генерация {equipParamsCount} параметров оборудования...");
            // Параметры оборудования
            var equipParams = new List<EquipParam>();
            for (int i = 1; i <= equipParamsCount; i++)
            {
                equipParams.Add(new EquipParam
                {
                    Name = $"Параметр_{i}",
                    UnitMeasure = _rnd.Next(0, 1) == 0 ? "ms" : "MB"
                });
            }
            await equipParamRepo.CreateMany(equipParams);
            Console.WriteLine($"✓ Создано {equipParams.Count} параметров");

            Console.WriteLine($"Генерация {testsCount} A/B тестов...");
            // A/B тесты
            var tests = new List<ABTests>();
            for (int i = 1; i <= testsCount; i++)
            {
                tests.Add(new ABTests
                {
                    ApplicationId = apps[_rnd.Next(apps.Count)].Id,
                    Name = $"Тест_{i}",
                    Description = $"Описание A/B теста {i}",
                    Enabled = _rnd.Next(0, 2) == 1
                });
            }
            await abTestRepo.CreateMany(tests);
            Console.WriteLine($"✓ Создано {tests.Count} A/B тестов");

            Console.WriteLine($"Генерация вариантов ({variantsPerTest} на тест)...");
            // Варианты
            var variants = new List<Variants>();
            foreach (var test in tests)
            {
                variants.Add(new Variants
                {
                    AbTestId = test.Id,
                    Name = "Control",
                    Description = "Контрольная группа",
                    Audience = 50,
                    Mean = _rnd.Next(10, 80)
                });

                for (int v = 2; v <= variantsPerTest; v++)
                {
                    variants.Add(new Variants
                    {
                        AbTestId = test.Id,
                        Name = $"Вариант_{(char)('A' + v - 1)}",
                        Description = $"Вариант {v} теста",
                        Audience = 50 / variantsPerTest,
                        Mean = _rnd.Next(10, 80)
                    });
                }
            }
            await variantRepo.CreateMany(variants);
            Console.WriteLine($"✓ Создано {variants.Count} вариантов");

            Console.WriteLine($"Генерация экземпляров ({instancesPerApp} на приложение)...");
            // Экземпляры
            var instances = new List<Instances>();
            foreach (var app in apps)
            {
                for (int i = 1; i <= instancesPerApp; i++)
                {
                    instances.Add(new Instances
                    {
                        ApplicationId = app.Id,
                        Version = i,
                        Name = $"{app.Name} v{i}",
                        Date = DateTime.UtcNow.AddDays(-_rnd.Next(1, 365))
                    });
                }
            }
            await instanceRepo.CreateMany(instances);
            Console.WriteLine($"✓ Создано {instances.Count} экземпляров");

            Console.WriteLine($"Генерация значений для оборудования...");
            // Значения оборудования
            var values = new List<Values>();
            foreach (var instance in instances)
            {
                foreach (var param in equipParams)
                {
                    values.Add(new Values
                    {
                        InstanceId = instance.Id,
                        ParamId = param.Id,
                        MetricValue = Math.Round(_rnd.NextDouble() * 100, 2)
                    });
                }
            }
            await valueRepo.CreateMany(values);
            Console.WriteLine($"✓ Создано {values.Count} значений");

            Console.WriteLine($"Генерация результатов ({resultsPerInstance} на экземпляр)...");
            // Результаты
            var results = new List<AbResults>();
            foreach (var instance in instances)
            {
                for (int i = 0; i < resultsPerInstance; i++)
                {
                    var testForInstance = tests[_rnd.Next(tests.Count)];
                    var variantsForTest = variants.Where(v => v.AbTestId == testForInstance.Id).ToList();

                    if (variantsForTest.Any())
                    {
                        results.Add(new AbResults
                        {
                            InstanceId = instance.Id,
                            VariantId = variantsForTest[_rnd.Next(variantsForTest.Count)].Id
                        });
                    }
                }
            }
            await resultRepo.CreateMany(results);
            Console.WriteLine($"✓ Создано {results.Count} результатов");

            // Итоговая статистика
            Console.WriteLine("\n=== ИТОГОВАЯ СТАТИСТИКА ===");
            Console.WriteLine($"Роли: {roles.Count}");
            Console.WriteLine($"Разработчики: {developers.Count}");
            Console.WriteLine($"Связи разработчиков: {devRoleApps.Count}");
            Console.WriteLine($"Приложения: {apps.Count}");
            Console.WriteLine($"Типы метрик: {metricTypes.Count}");
            Console.WriteLine($"Метрики: {metrics.Count}");
            Console.WriteLine($"Параметры оборудования: {equipParams.Count}");
            Console.WriteLine($"A/B тесты: {tests.Count}");
            Console.WriteLine($"Варианты: {variants.Count}");
            Console.WriteLine($"Экземпляры: {instances.Count}");
            Console.WriteLine($"Значения: {values.Count}");
            Console.WriteLine($"Результаты: {results.Count}");
            Console.WriteLine("=========================");
            Console.WriteLine("Генерация завершена успешно!");
        }

        private static async Task ClearAll(
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
            Console.WriteLine("Очистка коллекций...");
            await resultRepo.DeleteAll();
            await valueRepo.DeleteAll();
            await variantRepo.DeleteAll();
            await abTestRepo.DeleteAll();
            await equipParamRepo.DeleteAll();
            await instanceRepo.DeleteAll();
            await metricRepo.DeleteAll();
            await metricTypeRepo.DeleteAll();
            await devRoleAppRepo.DeleteAll();
            await appRepo.DeleteAll();
            await developerRepo.DeleteAll();
            await roleRepo.DeleteAll();
            Console.WriteLine("Очистка завершена");
        }
    }
}