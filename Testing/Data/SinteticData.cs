using MongoDB.Bson;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing.Data
{
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

            // Роли
            var roles = new List<Roles>
            {
                new() { Name = "Backend Dev" },
                new() { Name = "Frontend Dev" },
                new() { Name = "QA Engineer" },
                new() { Name = "DevOps" },
                new() { Name = "Data Analyst" },
                new() { Name = "Project Manager" },
                new() { Name = "Architect" },
                new() { Name = "Mobile Dev" },
                new() { Name = "ML Engineer" },
                new() { Name = "UX Designer" }
            };

            await roleRepo.CreateMany(roles);

 
            // Разрабы

            var developers = Enumerable.Range(1, 15)
                .Select(i => new Developers
                {
                    Login = $"dev_{i}",
                    PasswordHash = Guid.NewGuid().ToString()
                })
                .ToList();

            await developerRepo.CreateMany(developers);

            var apps = new List<Applications>
            {
    // Приложения

             new() { Name = "МагазинПро", Description = "Платформа для онлайн-торговли" },
             new() { Name = "АналитикаПлюс", Description = "BI и аналитическая панель" },
             new() { Name = "ВидеоПоток", Description = "Платформа для стриминга видео" },
             new() { Name = "НеоБанк", Description = "Цифровой банковский сервис" },
             new() { Name = "УчисьЛегко", Description = "Онлайн образовательная платформа" },
             new() { Name = "КлиентПро CRM", Description = "Система управления клиентами" },
             new() { Name = "HR Центр", Description = "Платформа для найма сотрудников" },
             new() { Name = "МаркетПлейс", Description = "Мультивендорная торговая площадка" },
             new() { Name = "Логистика Онлайн", Description = "Система отслеживания доставок" },
             new() { Name = "СоцСеть+", Description = "Социальная сеть нового поколения" },

    // Игры

        
             new() { Name = "Арена Битвы", Description = "PvP мультиплеерная арена" },
             new() { Name = "Неон Дрифт", Description = "Киберпанк гоночная игра" },
             new() { Name = "Осада Королевства", Description = "Стратегия в реальном времени" },
             new() { Name = "Космос X", Description = "Исследование космоса" },
             new() { Name = "Зомби Апокалипсис", Description = "Выживание в мире зомби" },
             new() { Name = "Ферма Пикселей", Description = "Казуальный симулятор фермы" },
             new() { Name = "Теневой Протокол", Description = "Стелс-экшен игра" },
             new() { Name = "Легенды Подземелий", Description = "RPG подземелья и лут" },
             new() { Name = "Танковые Войны Онлайн", Description = "Мультиплеерные танковые бои" },
             new() { Name = "Летающие Острова", Description = "Открытый мир приключений" }
            };

            await appRepo.CreateMany(apps);

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
                new() { Name = "Удержание" },
                new() { Name = "Конверсия" },
                new() { Name = "CTR" },
                new() { Name = "Среднее время сессии" },
                new() { Name = "Ошибки" },
                new() { Name = "Латентность" },
                new() { Name = "Активные пользователи" }
            };

            await metricTypeRepo.CreateMany(metricTypes);

            // Метрики
            var metrics = new List<Metrics>();

            foreach (var app in apps)
            {
                var selected = metricTypes.OrderBy(_ => _rnd.Next()).Take(6);

                foreach (var metricType in selected)
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
                        Date = DateTime.UtcNow.AddDays(-_rnd.Next(1, 100))
                    });
                }
            }

            await instanceRepo.CreateMany(instances);

            // Параметры оборудования

            var equipParams = new List<EquipParam>
            {
                new() { Name = "RAM", UnitMeasure = "GB" },
                new() { Name = "CPU", UnitMeasure = "MHz" },
                new() { Name = "Screen", UnitMeasure = "px" },
                new() { Name = "Storage", UnitMeasure = "GB" },
                new() { Name = "Battery", UnitMeasure = "mAh" },
                new() { Name = "Temperature", UnitMeasure = "C" },
                new() { Name = "Network", UnitMeasure = "Mbps" }
            };

            await equipParamRepo.CreateMany(equipParams);

            // AB тесты
            var tests = new List<ABTests>
            {
                new() { Name = "button_color", Description = "CTA test", Enabled = true },
                new() { Name = "checkout", Description = "Payment flow", Enabled = true },
                new() { Name = "header", Description = "Navigation", Enabled = true },
                new() { Name = "pricing", Description = "Pricing page", Enabled = true },
                new() { Name = "signup", Description = "Registration", Enabled = true },
                new() { Name = "hero_banner", Description = "Главный баннер / Hero section test", Enabled = true },
                new() { Name = "search_ui", Description = "Поиск и UX поиска", Enabled = true },
                new() { Name = "recommendation_block", Description = "Блок рекомендаций", Enabled = true },
                new() { Name = "cart_flow", Description = "Корзина и путь покупки", Enabled = true },
                new() { Name = "login_flow", Description = "Авторизация пользователей", Enabled = true },

                new() { Name = "landing_page", Description = "Landing page conversion test (лендинг)", Enabled = true },
                new() { Name = "dark_mode", Description = "Темная тема интерфейса", Enabled = true },
                new() { Name = "push_notifications", Description = "Push уведомления", Enabled = true },
                new() { Name = "onboarding", Description = "Онбординг новых пользователей", Enabled = true },
                new() { Name = "ads_layout", Description = "Расположение рекламы", Enabled = true },

                new() { Name = "product_card", Description = "Карточка товара UI/UX", Enabled = true },
                new() { Name = "filter_system", Description = "Фильтры и сортировка", Enabled = true },
                new() { Name = "video_autoplay", Description = "Автовоспроизведение видео", Enabled = true },
                new() { Name = "gamification", Description = "Геймификация интерфейса", Enabled = true },
                new() { Name = "pricing_ru", Description = "Тарифы (русская локализация)", Enabled = true }
            };

            await abTestRepo.CreateMany(tests);

            // Варианты

            var variants = new List<Variants>();

            foreach (var test in tests)
            {
                variants.AddRange(new[]
                {
                    new Variants { AbTestId = test.Id, Name = "A", Description = "Control", Audience = 40, Mean = _rnd.Next(10, 80) },
                    new Variants { AbTestId = test.Id, Name = "B", Description = "Variant", Audience = 40, Mean = _rnd.Next(10, 80) },
                    new Variants { AbTestId = test.Id, Name = "C", Description = "Alt", Audience = 20, Mean = _rnd.Next(10, 80) }
                });
            }

            await variantRepo.CreateMany(variants);

            // Результаты
            var results = new List<AbResults>();

            foreach (var instance in instances)
            {
                foreach (var variant in variants)
                {
                    int users = _rnd.Next(15, 21); // 15–20 USERS

                    for (int i = 0; i < users; i++)
                    {
                        results.Add(new AbResults
                        {
                            InstanceId = instance.Id,
                            VariantId = variant.Id
                        });
                    }
                }
            }

            await resultRepo.CreateMany(results);

            Console.WriteLine("Генерация завершена успешно");
        }
    }
}