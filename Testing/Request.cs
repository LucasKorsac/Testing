using MongoDB.Bson;
using System.Linq.Expressions;
using static Testing.Base.BaseMongo;

namespace Testing
{
    public static class Request
    {
        /// <summary> Фильтр вариантов по тесту </summary>
        public static Expression<Func<Variants, bool>> ByTest(string testId)
        {
            if (!ObjectId.TryParse(testId, out var objectId))
                return x => false;

            return x => x.AbTestId == objectId;
        }

        /// <summary> Только активные приложения </summary>
        public static Expression<Func<Applications, bool>>
            ActiveApps()
        {
            return x => x.Name != "";
        }

        /// <summary> Метрики по типу </summary>
        public static Expression<Func<Metrics, bool>>
            ByMetricType(string metricTypeId)
        {
            if (!ObjectId.TryParse(metricTypeId, out var objectId))
                throw new ArgumentException("Invalid metric type id");

            return x => x.MetricTypeId == objectId;
        }
    }
}