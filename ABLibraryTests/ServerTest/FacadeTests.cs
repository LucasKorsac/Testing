//using MongoDB.Driver;
//using Testing.Base;
//using Testing.Pattern;
//using static Testing.Base.BaseMongo;

//private readonly MongoClient _client;
//private readonly IMongoDatabase _database;
//private readonly Facade _facade;

//public FacadeTests()
//{
//    var connectionString =
//        Environment.GetEnvironmentVariable(
//            "MONGODB_TEST_CONNECTION_STRING")
//        ?? "mongodb://localhost:27017";

//    _client = new MongoClient(connectionString);

//    var databaseName =
//        $"ABTesting_FacadeTests_{Guid.NewGuid():N}";

//    _database = _client.GetDatabase(databaseName);

//    var abTestRepo = new MongoRepo<ABTests>(_database);
//    var variantRepo = new MongoRepo<Variants>(_database);
//    var resultRepo = new MongoRepo<AbResults>(_database);
//    var applicationRepo = new MongoRepo<Applications>(_database);
//    var instanceRepo = new MongoRepo<Instances>(_database);

//    var metricRepo = new MongoRepo<Metrics>(_database);
//    var metricTypeRepo = new MongoRepo<MetricTypes>(_database);
//    var roleRepo = new MongoRepo<Roles>(_database);
//    var developerRepo = new MongoRepo<Developers>(_database);
//    var devRoleRepo = new MongoRepo<DevelopRoleApplic>(_database);
//    var equipParamRepo = new MongoRepo<EquipParam>(_database);
//    var valueRepo = new MongoRepo<Values>(_database);

//    _facade = new Facade(
//        abTestRepo,
//        variantRepo,
//        resultRepo,
//        instanceRepo,
//        applicationRepo,
//        devRoleRepo,
//        metricRepo,
//        metricTypeRepo,
//        roleRepo,
//        developerRepo,
//        equipParamRepo,
//        valueRepo);
//}

//public void Dispose()
//{
//    _client.DropDatabase(_database.DatabaseNamespace.DatabaseName);
//}