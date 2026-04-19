using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;
//using static Testing.Interf;

namespace Testing
{
    /// <summary>
    /// Сервис для работы с A/B тестами
    /// </summary>
    internal class TestService
    {
        private readonly Facade _facade;
        private readonly IStrategy<Variants> _strategy;
        private readonly IMongoRepo<Variants> _variantRepo;

        public TestService(Facade facade, IStrategy<Variants> strategy, IMongoRepo<Variants> variantRepo)
        {
            _facade = facade;
            _strategy = strategy;
            _variantRepo = variantRepo;
        }

        /// <summary>
        /// Получение значений A/B теста для приложения
        /// </summary>
        public async Task<Dictionary<string, string>> GetAB(ObjectId applicationId)
        {
            var result = new Dictionary<string, string>();

           // var tests = await _facade.GetByApplication(applicationId);

            //foreach (var test in tests)
            //{
            //    var variants = await _variantRepo.Where(x => x.AbTestId == test.Id);

            //    if (variants.Count == 0)
            //        continue;

            //    var selected = _strategy.Choose(variants, variants[0]);

            //    result[test.Name] = selected.Name;
            //}

            return result;
        }
    }
}