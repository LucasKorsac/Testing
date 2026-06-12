using MongoDB.Bson;
using Testing.Base; 
using Testing.DTO;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

//namespace Testing
//{
//    /// <summary> Адаптивный сервис построения пула вариантов </summary>
//    public class Adaptation
//    {
//        private readonly IStatsBuilder _statsBuilder;
//        private readonly IWeightStrategy _weightStrategy;
//        private readonly IMongoRepo<Values> _valueRepo;
//        private readonly IMongoRepo<EquipParam> _paramRepo;

//        public Adaptation(IStatsBuilder statsBuilder, IWeightStrategy weightStrategy, 
//            IMongoRepo<Values> valueRepo, IMongoRepo<EquipParam> paramRepo)
//        {
//            _statsBuilder = statsBuilder;
//            _weightStrategy = weightStrategy;
//            _valueRepo = valueRepo;
//            _paramRepo = paramRepo;
//        }

//        /// <summary> Построение вероятностного пула вариантов </summary>
//        public async Task<List<VariantDto>> BuildPool(
//            ObjectId testId,
//            int minCount = 5)
//        {
//            var stats = await _statsBuilder.Build(testId);

//            if (stats.Count == 0)
//                return new List<VariantDto>();

//            // фильтр по минимальной выборке
//            stats = stats
//                .Where(s => s.Count >= minCount)
//                .OrderByDescending(s => s.Average)
//                .ToList();

//            if (stats.Count == 0)
//                return new List<VariantDto>();

//            int total = stats.Count;

//            var pool = new List<VariantDto>();

//            for (int i = 0; i < stats.Count; i++)
//            {
//                var s = stats[i];

//                int weight = _weightStrategy.CalculateWeight(
//                    index: i + 1,
//                    count: s.Count,
//                    total: total,
//                    average: s.Average);

//                for (int j = 0; j < weight; j++)
//                    pool.Add(s.Variant);
//            }

//            return pool;
//        }
//    }
//}

namespace Testing
{
    public class Adaptation
    {
        private readonly IStatsBuilder _statsBuilder;
        private readonly IWeightStrategy _weightStrategy;
        private readonly IMongoRepo<Values> _valueRepo;
        private readonly IMongoRepo<EquipParam> _paramRepo;

        public Adaptation(
            IStatsBuilder statsBuilder,
            IWeightStrategy weightStrategy,
            IMongoRepo<Values> valueRepo,
            IMongoRepo<EquipParam> paramRepo)
        {
            _statsBuilder = statsBuilder;
            _weightStrategy = weightStrategy;
            _valueRepo = valueRepo;
            _paramRepo = paramRepo;
        }

        /// <summary> Расчёт T-оценки пользователя по формуле </summary>
        private async Task<double> CalculateUserTScore(
            string instanceId,
            double wRetention = 1.0,
            double wSurvey = 1.0)
        {
            // Получаем ID параметров по их именам
            var params_ = await _paramRepo.Where(p =>
                p.Name == "days_in_game" ||
                p.Name == "rewards" ||
                p.Name == "survey_score");

            var daysParam = params_.FirstOrDefault(p => p.Name == "days_in_game");
            var rewardsParam = params_.FirstOrDefault(p => p.Name == "rewards");
            var surveyParam = params_.FirstOrDefault(p => p.Name == "survey_score");

            double D = 0, S = 0, maxOp = 0;

            // Дни в игре (D)
            if (daysParam != null)
            {
                var daysValue = await _valueRepo.FirstOrDefault(v =>
                    v.InstanceId == ObjectId.Parse(instanceId) &&
                    v.ParamId == daysParam.Id);
                if (daysValue != null) D = daysValue.MetricValue;
            }

            // Награды (S)
            if (rewardsParam != null)
            {
                var rewardsValue = await _valueRepo.FirstOrDefault(v =>
                    v.InstanceId == ObjectId.Parse(instanceId) &&
                    v.ParamId == rewardsParam.Id);
                if (rewardsValue != null) S = rewardsValue.MetricValue;
            }

            // Max(Op) - максимальная оценка опроса
            if (surveyParam != null)
            {
                var surveyValues = await _valueRepo.Where(v =>
                    v.ParamId == surveyParam.Id);
                if (surveyValues.Any())
                    maxOp = surveyValues.Max(v => v.MetricValue);
            }

            // T = √D × w_retention + √S + (Max(Op)/2) × w_survey
            double tScore = Math.Sqrt(D) * wRetention + Math.Sqrt(S) + (maxOp / 2.0) * wSurvey;

            return tScore;
        }

        /// <summary> Построение вероятностного пула вариантов </summary>
        public async Task<List<VariantDto>> BuildPool(
            ObjectId testId,
            string instanceId,  // ← добавили параметр
            int minCount = 5,
            double wRetention = 1.0,
            double wSurvey = 1.0)
        {
            var stats = await _statsBuilder.Build(testId);

            if (stats.Count == 0)
                return new List<VariantDto>();

            stats = stats
                .Where(s => s.Count >= minCount)
                .OrderByDescending(s => s.Average)
                .ToList();

            if (stats.Count == 0)
                return new List<VariantDto>();

            int total = stats.Count;

            // Рассчитываем T-оценку для пользователя
            double userTScore = await CalculateUserTScore(instanceId, wRetention, wSurvey);

            var pool = new List<VariantDto>();

            for (int i = 0; i < stats.Count; i++)
            {
                var s = stats[i];

                // Используем userTScore вместо average
                int weight = _weightStrategy.CalculateWeight(index: i + 1, count: s.Count, 
                    total: total, average: userTScore);

                for (int j = 0; j < weight; j++)
                    pool.Add(s.Variant);
            }

            return pool;
        }
    }
}