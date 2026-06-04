using Microsoft.AspNetCore.Mvc;
using WebAppTest.Control;

namespace WebAppTest.Control
{
    [ApiController]
    [Route("api/charts")]
    public class ChartApiController : ControllerBase
    {
        private readonly IUiService _ui;

        public ChartApiController(IUiService ui)
        {
            _ui = ui;
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetChartData([FromQuery] string metric = "tests")
        {
            var tests = await _ui.GetTestsAsync();
            var variants = await _ui.GetAllVariantsAsync();
            var applications = await _ui.GetApplicationsAsync();

            // Группировка по месяцам с использованием эвристики (по ID)
            // или просто возвращаем общее количество без группировки по датам
            var testsCount = tests.Count;
            var variantsCount = variants.Count;
            var applicationsCount = applications.Count;

            // Для демонстрации создадим фиктивные даты (последние 6 месяцев)
            var dates = new List<string>();
            var testsValues = new List<int>();
            var variantsValues = new List<int>();
            var applicationsValues = new List<int>();

            for (int i = 5; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                dates.Add(date.ToString("yyyy-MM"));

                // Распределяем существующие данные по месяцам (для демонстрации)
                testsValues.Add(i == 0 ? testsCount : new Random().Next(1, testsCount / 2));
                variantsValues.Add(i == 0 ? variantsCount : new Random().Next(1, variantsCount / 2));
                applicationsValues.Add(i == 0 ? applicationsCount : new Random().Next(1, applicationsCount / 2));
            }

            return Ok(new
            {
                dates = dates,
                testsValues = testsValues,
                variantsValues = variantsValues,
                applicationsValues = applicationsValues
            });
        }
    }
}