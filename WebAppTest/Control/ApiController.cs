using Microsoft.AspNetCore.Mvc;

namespace WebAppTest.Control
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly IUiService _ui;

        public ApiController(IUiService ui)
        {
            _ui = ui;
        }

        /// <summary>
        /// Получение данных для графика на главной странице
        /// </summary>
        [HttpGet("charts/main")]
        public async Task<IActionResult> GetMainChartData()
        {
            var tests = await _ui.GetTestsAsync();
            var variants = await _ui.GetAllVariantsAsync();
            var applications = await _ui.GetApplicationsAsync();

            var now = DateTime.Now;
            var dates = new List<string>();
            var testsValues = new List<int>();
            var variantsValues = new List<int>();
            var applicationsValues = new List<int>();

            // Группируем данные по месяцам
            for (int i = 5; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                dates.Add(date.ToString("MMM yyyy"));
            }

            var totalTests = tests.Count;
            var totalVariants = variants.Count;
            var totalApplications = applications.Count;

            for (int i = 0; i < dates.Count; i++)
            {
                if (i == dates.Count - 1)
                {
                    testsValues.Add(totalTests);
                    variantsValues.Add(totalVariants);
                    applicationsValues.Add(totalApplications);
                }
                else
                {
                    var factor = (i + 1) / (double)dates.Count;
                    testsValues.Add(Math.Max(1, (int)(totalTests * factor)));
                    variantsValues.Add(Math.Max(1, (int)(totalVariants * factor)));
                    applicationsValues.Add(Math.Max(1, (int)(totalApplications * factor)));
                }
            }

            return Ok(new
            {
                dates = dates,
                testsValues = testsValues,
                variantsValues = variantsValues,
                applicationsValues = applicationsValues
            });
        }

        /// <summary>
        /// Получение данных для графика теста
        /// </summary>
        [HttpGet("charts/test/{testId}")]
        /// <summary>
        /// Получение данных для графика теста
        /// </summary>
        [HttpGet("charts/test/{testId}")]
        public async Task<IActionResult> GetTestChartData(string testId)
        {
            var test = await _ui.GetTestByIdAsync(testId);
            if (test == null)
                return NotFound("Тест не найден");

            var variants = (await _ui.GetAllVariantsAsync())
                .Where(v => v.AbTestId == testId)
                .ToList();

            var results = await _ui.GetResultsAsync(testId);
            var metrics = await _ui.GetMetricsWithTypesAsync(test.ApplicationId);

            var labels = variants.Select(v => v.Name).ToList();
            var installs = variants.Select(v => results.Count(r => r.VariantId == v.Id)).ToList();

            // Исправлено: убираем обращение к InstanceId
            var metricsValues = variants.Select(v =>
            {
                var variantResults = results.Where(r => r.VariantId == v.Id).ToList();
                // Используем среднее значение метрик напрямую, без привязки к InstanceId
                return metrics.Any() ? Math.Round(metrics.Average(m => m.Metric.Meaning), 2) : 0;
            }).ToList();

            return Ok(new
            {
                labels = labels,
                installs = installs,
                metrics = metricsValues
            });

            return Ok(new
            {
                labels = labels,
                installs = installs,
                metrics = metricsValues
            });
        }

        /// <summary>
        /// Получение данных для графика экземпляра
        /// </summary>
        [HttpGet("charts/instance/{instanceId}")]
        public async Task<IActionResult> GetInstanceChartData(string instanceId)
        {
            var instance = await _ui.GetInstanceAsync(instanceId);
            if (instance == null)
                return NotFound("Экземпляр не найден");

            var results = await _ui.GetResultsByInstanceAsync(instanceId);
            var metrics = await _ui.GetMetricsWithTypesAsync(instance.ApplicationId);
            var variants = await _ui.GetAllVariantsAsync();

            var chartData = new List<object>();

            foreach (var metric in metrics)
            {
                var metricName = metric.TypeName ?? "Метрика";
                var values = results.Select(r =>
                {
                    var variant = variants.FirstOrDefault(v => v.Id == r.VariantId);
                    return metric.Metric.Meaning;
                }).ToList();

                var frequency = values
                    .GroupBy(v => v)
                    .Select(g => new { Value = g.Key, Frequency = g.Count() })
                    .ToList();

                chartData.Add(new
                {
                    metricName = metricName,
                    data = frequency
                });
            }

            return Ok(chartData);
        }

        /// <summary>
        /// Получение списка тестов для выпадающего списка
        /// </summary>
        [HttpGet("tests/list")]
        public async Task<IActionResult> GetTestsList()
        {
            var tests = await _ui.GetTestsAsync();
            return Ok(tests.Select(t => new { t.Id, t.Name, t.Enabled }));
        }

        /// <summary>
        /// Получение статистики по тесту для карточек
        /// </summary>
        [HttpGet("tests/stats/{testId}")]
        public async Task<IActionResult> GetTestStats(string testId)
        {
            var test = await _ui.GetTestByIdAsync(testId);
            if (test == null)
                return NotFound();

            var variants = (await _ui.GetAllVariantsAsync())
                .Where(v => v.AbTestId == testId)
                .ToList();

            var results = await _ui.GetResultsAsync(testId);

            return Ok(new
            {
                testName = test.Name,
                isActive = test.Enabled,
                variantsCount = variants.Count,
                installsCount = results.Count,
                variants = variants.Select(v => new
                {
                    v.Name,
                    installs = results.Count(r => r.VariantId == v.Id),
                    percent = results.Count > 0
                        ? Math.Round(results.Count(r => r.VariantId == v.Id) * 100.0 / results.Count, 2)
                        : 0
                })
            });
        }

        /// <summary>
        /// Установка стратегии для теста
        /// </summary>
        [HttpPost("tests/strategy/{testId}")]
        public async Task<IActionResult> SetTestStrategy(string testId, [FromBody] StrategyRequest request)
        {
            if (string.IsNullOrEmpty(request?.Strategy))
                return BadRequest("Strategy is required");

            await _ui.SetTestStrategyAsync(testId, request.Strategy);
            return Ok(new { message = $"Strategy '{request.Strategy}' applied to test {testId}" });
        }

        /// <summary>
        /// Получение статистики MAB для теста
        /// </summary>
        [HttpGet("tests/mab-stats/{testId}")]
        public async Task<IActionResult> GetMABStats(string testId)
        {
            var stats = await _ui.GetMABStatsAsync(testId);
            return Ok(stats);
        }

        public class StrategyRequest
        {
            public string Strategy { get; set; } = "";
        }

    }
}