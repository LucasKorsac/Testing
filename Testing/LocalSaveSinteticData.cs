using Testing.Base;
using Testing.Data;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing
{
    /// <summary> Локальное заполнение MongoDB синтетическими данными </summary>
    public class LocalSaveSinteticData
    {
        private readonly IMongoFactory _factory;

        public LocalSaveSinteticData(IMongoFactory factory)
        {
            _factory = factory;
        }

        public async Task Run()
        {
            Console.WriteLine("Starting MongoDB seed...");

            var abTestRepo = _factory.Create<ABTests>();
            var variantRepo = _factory.Create<Variants>();
            var resultRepo = _factory.Create<AbResults>();
            var valueRepo = _factory.Create<Values>();
            var instanceRepo = _factory.Create<Instances>();
            var metricRepo = _factory.Create<Metrics>();

            var roleRepo = _factory.Create<Roles>();
            var devRepo = _factory.Create<Developers>();
            var devRoleRepo = _factory.Create<DevelopRoleApplic>();
            var appRepo = _factory.Create<Applications>();
            var metricTypeRepo = _factory.Create<MetricTypes>();
            var equipRepo = _factory.Create<EquipParam>();

            // Очистка ВСЕЙ базы (важно для консистентности)
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

            // защита от пустого seed
            await SinteticData.Init(roleRepo, devRepo, devRoleRepo, appRepo, metricTypeRepo, metricRepo,
                instanceRepo, equipRepo, valueRepo, abTestRepo, variantRepo, resultRepo);

            Console.WriteLine("MongoDB seed completed");
        }
    }
}