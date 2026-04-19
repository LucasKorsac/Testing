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
        /// <summary>
        /// Фильтр тестов по приложению
        /// </summary>
        //public static Expression<Func<ABTests, bool>> ByApplication(string appId)
        //{
        //    var objectId = MongoDB.Bson.ObjectId.Parse(appId);
        //    return x => x.ApplicationId == objectId;
        //}

        /// <summary>
        /// Фильтр вариантов по тесту
        /// </summary>
        public static Expression<Func<Variants, bool>> ByTest(string testId)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(testId);
            return x => x.AbTestId == objectId;
        }

        /// <summary>
        /// Только активные приложения (пример расширения логики)
        /// </summary>
        public static Expression<Func<Applications, bool>> ActiveApps()
        {
            return x => x.Name != "";
        }

        /// <summary>
        /// Метрики по типу
        /// </summary>
        public static Expression<Func<Metrics, bool>> ByMetricType(string metricTypeId)
        {
            var objectId = MongoDB.Bson.ObjectId.Parse(metricTypeId);
            return x => x.MetricTypeId == objectId;
        }
    }
}
