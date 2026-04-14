using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Base;
using static Testing.Base.BaseMongo;
using static Testing.Interf;

namespace Testing.Pattern
{
    /// <summary>
    /// Паттерн фасад. Сокрытие работы с репозиториями
    /// </summary>
    public class Facade
    {
        private readonly IMongoRepo<AbTest> _repo;

        public Facade(IMongoRepo<AbTest> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Получение всех тестов
        /// </summary>
        public async Task<List<AbTest>> GetAll()
        {
            return await _repo.Get(x => true);
        }

        /// <summary>
        /// Получение тестов по приложению
        /// </summary>
        public async Task<List<AbTest>> GetByApplication(ObjectId appId)
        {
            return await _repo.Get(x => x.ApplicationId == appId);
        }
    }
}
