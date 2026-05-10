using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    /// <summary> Реализация доступа к Mongo репозиториям </summary>
    public class MongoDataAccess : IDataAccess
    {
        public IMongoRepo<ABTests> AbTests { get; }
        public IMongoRepo<Variants> Variants { get; }
        public IMongoRepo<AbResults> Results { get; }
        public IMongoRepo<Instances> Instances { get; }
        public IMongoRepo<Values> Values { get; }

        public MongoDataAccess(IMongoFactory factory)
        {
            AbTests = factory.Create<ABTests>();
            Variants = factory.Create<Variants>();
            Results = factory.Create<AbResults>();
            Instances = factory.Create<Instances>();
            Values = factory.Create<Values>();
        }
    }
}