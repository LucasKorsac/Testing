using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Testing.Base.BaseMongo;

namespace Testing
{
    public class Request
    {
        /// <summary> Фильтр вариантов по тесту </summary>
        public static Expression<Func<Variants, bool>> ByTest(string testId)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(testId);
            return x => x.AbTestId == objectId;
        }

        /// <summary> Только активные приложения </summary>
        public static Expression<Func<Applications, bool>> ActiveApps()
        {
            return x => x.Name != "";
        }

        /// <summary> Метрики по типу </summary>
        public static Expression<Func<Metrics, bool>> ByMetricType(string metricTypeId)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(metricTypeId);
            return x => x.MetricTypeId == objectId;
        }
    }
}
