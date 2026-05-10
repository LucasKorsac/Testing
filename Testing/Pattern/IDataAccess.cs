using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    /// <summary>
    /// Единый доступ ко всем Mongo репозиториям (замена ручных параметров в Facade)
    /// </summary>
    public interface IDataAccess
    {
        IMongoRepo<ABTests> AbTests { get; }
        IMongoRepo<Variants> Variants { get; }
        IMongoRepo<AbResults> Results { get; }
        IMongoRepo<Instances> Instances { get; }
        IMongoRepo<Values> Values { get; }
    }
}