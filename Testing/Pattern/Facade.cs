using ABLibrary.Models;
using MongoDB.Bson;
using System.Text;
using Testing.Base;
using Testing.DTO;
using System.Security.Cryptography;
using System.Text;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    /// <summary> Фасад для работы с A/B тестами, приложениями и аналитикой </summary>
    public class Facade
    {
        private readonly IMongoRepo<ABTests> _abTests;
        private readonly IMongoRepo<BaseMongo.Variants> _variants;
        private readonly IMongoRepo<AbResults> _results;
        private readonly IMongoRepo<Instances> _instances;
        private readonly IMongoRepo<Applications> _applications;
        private readonly IMongoRepo<DevelopRoleApplic> _devRoles;
        private readonly IMongoRepo<Metrics> _metrics;
        private readonly IMongoRepo<MetricTypes> _metricTypes;
        private readonly IMongoRepo<Roles> _roles;
        private readonly IMongoRepo<Developers> _developers;
        private readonly IMongoRepo<EquipParam> _equipParam;
        private readonly IMongoRepo<Values> _values;

        public Facade(
            IMongoRepo<ABTests> abTests,
            IMongoRepo<BaseMongo.Variants> variants,
            IMongoRepo<AbResults> results,
            IMongoRepo<Instances> instances,
            IMongoRepo<Applications> applications,
            IMongoRepo<DevelopRoleApplic> devRoles,
            IMongoRepo<Metrics> metrics,
            IMongoRepo<MetricTypes> metricTypes,
            IMongoRepo<Roles> roles,
            IMongoRepo<Developers> developers,
            IMongoRepo<EquipParam> equipParam,
            IMongoRepo<Values> value)
        {
            _abTests = abTests;
            _variants = variants;
            _results = results;
            _instances = instances;
            _applications = applications;
            _devRoles = devRoles;
            _metrics = metrics;
            _metricTypes = metricTypes;
            _roles = roles;
            _developers = developers;
            _equipParam = equipParam;
            _values = value;
        }

        private static string ToId(ObjectId id) => id.ToString();

        // Тесты

        public async Task<List<TestDto>> GetAllTests()
        {
            var tests = await _abTests.GetAll();

            return tests.Select(t => new TestDto
            {
                Id = ToId(t.Id),
                ApplicationId = ToId(t.ApplicationId),
                Name = t.Name,
                Description = t.Description,
                Enabled = t.Enabled
            }).ToList();
        }
        public async Task<TestDto?> GetById(ObjectId id)
        {
            var t = await _abTests.GetById(id);
            if (t == null) return null;

            return new TestDto
            {
                Id = ToId(t.Id),
                ApplicationId = ToId(t.ApplicationId),
                Name = t.Name,
                Description = t.Description,
                Enabled = t.Enabled
            };
        }
        public async Task<List<TestWithVariantsDto>> GetTests()
        {
            var tests = await _abTests.GetAll();
            var variants = await _variants.GetAll();

            var grouped = variants
                .GroupBy(v => v.AbTestId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<TestWithVariantsDto>();

            foreach (var test in tests)
            {
                grouped.TryGetValue(test.Id, out var list);

                result.Add(new TestWithVariantsDto
                {
                    Test = new TestDto
                    {
                        Id = ToId(test.Id),
                        ApplicationId = ToId(test.ApplicationId),
                        Name = test.Name,
                        Description = test.Description,
                        Enabled = test.Enabled
                    },
                    Variants = (list ?? new List<Variants>())
                        .Select(v => new VariantDto
                        {
                            Id = ToId(v.Id),
                            AbTestId = ToId(v.AbTestId),
                            Name = v.Name,
                            Description = v.Description,
                            Mean = v.Mean,
                            Audience = v.Audience
                        }).ToList()
                });
            }

            return result;
        }

        // Приложения

        public async Task<List<ApplicationDto>> GetApplications()
        {
            var apps = await _applications.GetAll();

            return apps.Select(a => new ApplicationDto
            {
                Id = ToId(a.Id),
                Name = a.Name,
                Description = a.Description
            }).ToList();
        }

        public async Task<ApplicationDto?> GetApplication(ObjectId id)
        {
            var a = await _applications.GetById(id);
            if (a == null) return null;

            return new ApplicationDto
            {
                Id = ToId(a.Id),
                Name = a.Name,
                Description = a.Description
            };
        }

        // Экземпляры

        public async Task<List<InstanceDto>> GetInstancesByApp(ObjectId appId)
        {
            var instances = await _instances.Where(x => x.ApplicationId == appId);

            return instances.Select(i => new InstanceDto
            {
                Id = ToId(i.Id),
                ApplicationId = ToId(i.ApplicationId),
                Version = i.Version,
                Name = i.Name,
                Date = i.Date
            }).ToList();
        }

        public async Task<InstanceDto?> GetInstance(ObjectId id)
        {
            var i = await _instances.GetById(id);
            if (i == null) return null;

            return new InstanceDto
            {
                Id = ToId(i.Id),
                ApplicationId = ToId(i.ApplicationId),
                Version = i.Version,
                Name = i.Name,
                Date = i.Date
            };
        }

        // Метрики 

        public async Task<List<MetricWithTypeDto>> GetMetricsWithTypes(ObjectId appId)
        {
            var metrics = await _metrics.Where(x => x.ApplicationId == appId);
            var types = await _metricTypes.GetAll();

            return metrics.Select(m =>
            {
                var type = types.FirstOrDefault(t => t.Id == m.MetricTypeId);

                return new MetricWithTypeDto
                {
                    Metric = new MetricDto
                    {
                        Id = ToId(m.Id),
                        ApplicationId = ToId(m.ApplicationId),
                        MetricTypeId = ToId(m.MetricTypeId),
                        Meaning = m.Meaning
                    },
                    TypeName = type?.Name
                };
            }).ToList();
        }

        // Варианты

        public async Task<List<DTO.VariantDto>> GetAllVariants()
        {
            var variants = await _variants.GetAll();

            return variants.Select(v => new VariantDto
            {
                Id = ToId(v.Id),
                AbTestId = ToId(v.AbTestId),
                Name = v.Name,
                Description = v.Description,
                Mean = v.Mean,
                Audience = v.Audience
            }).ToList();
        }

        // Результаты

        public async Task<List<IdDto>> GetResults(ObjectId testId)
        {
            var variants = await _variants.Where(v => v.AbTestId == testId);

            var ids = variants.Select(v => v.Id).ToList();

            var res = await _results.Where(r => ids.Contains(r.VariantId));

            return res.Select(r => new IdDto
            {
                Id = ToId(r.Id)
            }).ToList();
        }

        // Анализ

        public async Task<int> GetActiveTestsCount()
        {
            var tests = await _abTests.Where(x => x.Enabled);
            return tests.Count;
        }

        public async Task<int> GetTestsCount()
        {
            var tests = await _abTests.GetAll();
            return tests.Count;
        }

        public async Task<int> GetVariantsCount()
        {
            var variants = await _variants.GetAll();
            return variants.Count;
        }

        public async Task DeleteTest(string id)
        {
            var objectId = ObjectId.Parse(id);

            var variants = await _variants.Where(v => v.AbTestId == objectId);

            var variantIds = variants.Select(v => v.Id).ToList();

            await _results.DeleteMany(r => variantIds.Contains(r.VariantId));
            await _variants.DeleteMany(v => v.AbTestId == objectId);
            await _abTests.Delete(objectId);
        }

        public async Task StopTest(string id)
        {
            var test = await _abTests.GetById(ObjectId.Parse(id));
            if (test == null) return;

            test.Enabled = false;
            await _abTests.Update(test.Id, test);
        }

        public async Task ResumeTest(string id)
        {
            var test = await _abTests.GetById(ObjectId.Parse(id));
            if (test == null) return;

            test.Enabled = true;
            await _abTests.Update(test.Id, test);
        }
        public async Task<List<ApplicationWithInstancesDto>> GetApplicationsWithInstances()
        {
            var apps = await _applications.GetAll();
            var instances = await _instances.GetAll();

            var grouped = instances
                .GroupBy(i => i.ApplicationId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<ApplicationWithInstancesDto>();

            foreach (var app in apps)
            {
                grouped.TryGetValue(app.Id, out var list);

                result.Add(new ApplicationWithInstancesDto
                {
                    Application = new ApplicationDto
                    {
                        Id = app.Id.ToString(),
                        Name = app.Name,
                        Description = app.Description
                    },
                    Instances = (list ?? new List<Instances>())
                        .Select(i => new InstanceDto
                        {
                            Id = i.Id.ToString(),
                            ApplicationId = i.ApplicationId.ToString(),
                            Name = i.Name,
                            Version = i.Version,
                            Date = i.Date
                        }).ToList()
                });
            }

            return result;
        }
        public async Task<List<ValueDto>> GetDeviceValues(ObjectId appId)
        {
            var instances = await _instances.Where(x => x.ApplicationId == appId);
            var ids = instances.Select(i => i.Id).ToList();

            var values = await _values.Where(x => ids.Contains(x.InstanceId));

            return values.Select(v => new ValueDto
            {
                Id = v.Id.ToString(),
                InstanceId = v.InstanceId.ToString(),
                ParamId = v.ParamId.ToString(),
                MetricValue = v.MetricValue
            }).ToList();
        }
        public async Task<List<EquipParamDto>> GetEquipParam()
        {
            var data = await _equipParam.GetAll();

            return data.Select(x => new EquipParamDto
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                UnitMeasure = x.UnitMeasure
            }).ToList();
        }

        public async Task<List<AbResultsDto>> GetResultsByInstance(ObjectId instanceId)
        {
            var results = await _results.Where(r => r.InstanceId == instanceId);

            return results.Select(r => new AbResultsDto
            {
                Id = r.Id.ToString(),
                InstanceId = r.InstanceId.ToString(),
                VariantId = r.VariantId.ToString()
            }).ToList();
        }

        public async Task UpdateTest(TestDto dto)
        {
            var id = ObjectId.Parse(dto.Id);

            var test = await _abTests.GetById(id);
            if (test == null) return;

            test.Name = dto.Name;
            test.Description = dto.Description;
            test.Enabled = dto.Enabled;

            await _abTests.Update(test.Id, test);
        }

        // Результаты по тесту

        /// <summary> Получение результатов по тесту </summary>
        public async Task<List<AbResultsDto>> GetResultsByTest(ObjectId testId)
        {
            // получаем все варианты теста
            var variants = await _variants.Where(v => v.AbTestId == testId);

            // собираем их Id
            var variantIds = variants.Select(v => v.Id).ToList();

            // получаем результаты по вариантам
            var results = await _results.Where(r => variantIds.Contains(r.VariantId));

            // маппинг в DTO
            return results.Select(r => new AbResultsDto
            {
                Id = r.Id.ToString(),
                VariantId = r.VariantId.ToString(),
                InstanceId = r.InstanceId.ToString()
            }).ToList();
        }

        public async Task<List<AbResultsDto>> GetAllResults()
        {
            var results = await _results.GetAll();

            return results.Select(r => new AbResultsDto
            {
                Id = r.Id.ToString(),
                InstanceId = r.InstanceId.ToString(),
                VariantId = r.VariantId.ToString()
            }).ToList();
        }

        public async Task<List<InstanceDto>> GetInstances()
        {
            var instances = await _instances.GetAll();

            return instances.Select(i => new InstanceDto
            {
                Id = i.Id.ToString(),
                ApplicationId = i.ApplicationId.ToString(),
                Name = i.Name,
                Version = i.Version,
                Date = i.Date
            }).ToList();
        }

        public async Task DeleteVariant(string id)
        {
            var objectId = ObjectId.Parse(id);

            // удалить результаты варианта
            await _results.DeleteMany(r => r.VariantId == objectId);

            // удалить сам вариант
            await _variants.Delete(objectId);
        }

        public async Task DeleteApplication(string id)
        {
            var objectId = ObjectId.Parse(id);

            var instances =
                await _instances.Where(x =>
                    x.ApplicationId == objectId);

            var instanceIds =
                instances.Select(x => x.Id).ToList();

            await _values.DeleteMany(v =>
                instanceIds.Contains(v.InstanceId));

            await _metrics.DeleteMany(m =>
                m.ApplicationId == objectId);

            await _instances.DeleteMany(i =>
                i.ApplicationId == objectId);

            await _applications.Delete(objectId);
        }

        public async Task CreateTest(string applicationId, string name,string description)
        {
            var test = new ABTests
            {
                Id = ObjectId.GenerateNewId(),

                ApplicationId =
                    ObjectId.Parse(applicationId),

                Name = name,

                Description = description,

                Enabled = true
            };

            await _abTests.Create(test); 
        }

        //public async Task SaveEvent(TestEvent evt)
        //{
        //    // найти тест
        //    var tests =
        //        await _abTests.Where(x =>
        //            x.Name == evt.TestName);

        //    var test = tests.FirstOrDefault();

        //    if (test == null)
        //        return;

        //    // найти вариант
        //    var variants =
        //        await _variants.Where(x =>
        //            x.AbTestId == test.Id &&
        //            x.Name == evt.Variant);

        //    var variant =
        //        variants.FirstOrDefault();

        //    if (variant == null)
        //        return;

        //    // создать результат
        //    var result = new AbResults
        //    {
        //        Id = ObjectId.GenerateNewId(),

        //        VariantId = variant.Id,

        //        InstanceId = ObjectId.GenerateNewId()
        //    };

        //    await _results.Create(result);
        //}

        public async Task SaveEvent(TestEvent evt)
        {
            // найти тест по имени
            var tests = await _abTests.Where(x => x.Name == evt.TestName);
            var test = tests.FirstOrDefault();

            if (test == null)
                return;

            // найти вариант по тесту и имени варианта
            var variants = await _variants.Where(x => x.AbTestId == test.Id && x.Name == evt.Variant);
            var variant = variants.FirstOrDefault();

            if (variant == null)
                return;

            // наличие InstanceId
            if (string.IsNullOrEmpty(evt.InstanceId))
            {
                // Логируем ошибку, но не сохраняем результат
                Console.WriteLine($"SaveEvent: InstanceId is empty for event {evt.TestName}/{evt.Variant}");
                return;
            }

            // создаём результат с реальным InstanceId из события
            var result = new AbResults
            {
                Id = ObjectId.GenerateNewId(),
                VariantId = variant.Id,
                InstanceId = ObjectId.Parse(evt.InstanceId) 
            };

            await _results.Create(result);
        }


        /// <summary> Получение разработчика по логину </summary>
        public async Task<DeveloperDto?> GetDeveloperByLogin(string login)
        {
            var developer = await _developers.FirstOrDefault(x => x.Login == login);

            if (developer == null)
                return null;

            return new DeveloperDto
            {
                Id = developer.Id.ToString(),
                Login = developer.Login,
                PasswordHash = developer.PasswordHash
            };
        }

        /// <summary> Регистрация нового разработчика </summary>
        public async Task<bool> RegisterDeveloper(string login, string password)
        {
            // Проверяем, существует ли пользователь
            var existing = await _developers.FirstOrDefault(x => x.Login == login);
            if (existing != null)
                return false;

            // Создаем нового разработчика
            var developer = new Developers
            {
                Id = ObjectId.GenerateNewId(),
                Login = login,
                PasswordHash = HashPassword(password)
            };

            await _developers.Create(developer);
            return true;
        }

        /// <summary> Проверка пароля </summary>
        public bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        /// <summary> Хеширование пароля </summary>
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public async Task CreateVariant(string testId, string name, string description)
        {
            var variant = new Variants
            {
                Id = ObjectId.GenerateNewId(),
                AbTestId = ObjectId.Parse(testId),
                Name = name,
                Description = description,
                Mean = 0,
                Audience = 0
            };
            await _variants.Create(variant);
        }

        // ========== ПРИЛОЖЕНИЯ ==========

        /// <summary> Создание приложения </summary>
        public async Task CreateApplication(string name, string description)
        {
            var app = new Applications
            {
                Id = ObjectId.GenerateNewId(),
                Name = name,
                Description = description ?? ""
            };
            await _applications.Create(app);
        }

        /// <summary> Обновление приложения </summary>
        public async Task UpdateApplication(string id, string name, string description)
        {
            var objectId = ObjectId.Parse(id);
            var app = await _applications.GetById(objectId);
            if (app == null) return;

            app.Name = name;
            app.Description = description ?? "";
            await _applications.Update(app.Id, app);
        }

        /// <summary> Создание экземпляра приложения </summary>
        public async Task CreateInstance(string applicationId, string name, int version)
        {
            var instance = new Instances
            {
                Id = ObjectId.GenerateNewId(),
                ApplicationId = ObjectId.Parse(applicationId),
                Name = name,
                Version = version,
                Date = DateTime.UtcNow
            };
            await _instances.Create(instance);
        }

        /// <summary> Обновление экземпляра </summary>
        public async Task UpdateInstance(string id, string name, int version)
        {
            var objectId = ObjectId.Parse(id);
            var instance = await _instances.GetById(objectId);
            if (instance == null) return;

            instance.Name = name;
            instance.Version = version;
            await _instances.Update(instance.Id, instance);
        }

        /// <summary> Удаление экземпляра </summary>
        public async Task DeleteInstance(string id)
        {
            var objectId = ObjectId.Parse(id);

            // Удаляем значения метрик экземпляра
            await _values.DeleteMany(v => v.InstanceId == objectId);

            // Удаляем результаты экземпляра
            await _results.DeleteMany(r => r.InstanceId == objectId);

            // Удаляем сам экземпляр
            await _instances.Delete(objectId);
        }
    }
}