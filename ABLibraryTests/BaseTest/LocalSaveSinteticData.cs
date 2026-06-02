using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace ABProjectTests.BaseTest
{
    public class LocalSaveSinteticData
    {
        private readonly IMongoFactory _factory;

        public LocalSaveSinteticData(IMongoFactory factory)
        {
            _factory = factory;
        }

        public async Task Run()
        {
            Console.WriteLine("=== START SEED ===");

            // Защита: только Development
            if (!IsDevelopment())
            {
                Console.WriteLine("Seed запрещён вне Development окружения");
                return;
            }

            var (abTestRepo,
                 variantRepo,
                 resultRepo,
                 valueRepo,
                 instanceRepo,
                 metricRepo,
                 roleRepo,
                 devRepo,
                 devRoleRepo,
                 appRepo,
                 metricTypeRepo,
                 equipRepo) = CreateRepositories();

            try
            {
                Console.WriteLine("Очистка базы данных...");

                await ClearAll(
                    roleRepo,
                    devRepo,
                    devRoleRepo,
                    appRepo,
                    metricTypeRepo,
                    metricRepo,
                    instanceRepo,
                    equipRepo,
                    valueRepo,
                    abTestRepo,
                    variantRepo,
                    resultRepo
                );

                Console.WriteLine("Заполнение базы...");

                Console.WriteLine("=== SEED COMPLETED SUCCESSFULLY ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Ошибка при заполнении базы: " + ex.Message);
                throw;
            }
        }

        private bool IsDevelopment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                   == "Development";
        }

        private (
            IMongoRepo<ABTests>,
            IMongoRepo<Variants>,
            IMongoRepo<AbResults>,
            IMongoRepo<Values>,
            IMongoRepo<Instances>,
            IMongoRepo<Metrics>,
            IMongoRepo<Roles>,
            IMongoRepo<Developers>,
            IMongoRepo<DevelopRoleApplic>,
            IMongoRepo<Applications>,
            IMongoRepo<MetricTypes>,
            IMongoRepo<EquipParam>
        ) CreateRepositories()
        {
            return (
                _factory.Create<ABTests>(),
                _factory.Create<Variants>(),
                _factory.Create<AbResults>(),
                _factory.Create<Values>(),
                _factory.Create<Instances>(),
                _factory.Create<Metrics>(),
                _factory.Create<Roles>(),
                _factory.Create<Developers>(),
                _factory.Create<DevelopRoleApplic>(),
                _factory.Create<Applications>(),
                _factory.Create<MetricTypes>(),
                _factory.Create<EquipParam>()
            );
        }

        private async Task ClearAll(
            IMongoRepo<Roles> roleRepo,
            IMongoRepo<Developers> devRepo,
            IMongoRepo<DevelopRoleApplic> devRoleRepo,
            IMongoRepo<Applications> appRepo,
            IMongoRepo<MetricTypes> metricTypeRepo,
            IMongoRepo<Metrics> metricRepo,
            IMongoRepo<Instances> instanceRepo,
            IMongoRepo<EquipParam> equipRepo,
            IMongoRepo<Values> valueRepo,
            IMongoRepo<ABTests> abTestRepo,
            IMongoRepo<Variants> variantRepo,
            IMongoRepo<AbResults> resultRepo)
        {
            Console.WriteLine(" Очистка коллекций...");

            await roleRepo.DeleteAll();
            await devRepo.DeleteAll();
            await devRoleRepo.DeleteAll();
            await appRepo.DeleteAll();
            await metricTypeRepo.DeleteAll();
            await metricRepo.DeleteAll();
            await instanceRepo.DeleteAll();
            await equipRepo.DeleteAll();
            await valueRepo.DeleteAll();
            await abTestRepo.DeleteAll();
            await variantRepo.DeleteAll();
            await resultRepo.DeleteAll();

            Console.WriteLine(" Очистка завершена");
        }
    }
}
