using Microsoft.AspNetCore.Mvc;
using System.Text;
using Testing.DTO;
using WebAppTest.Control;

namespace WebAppTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly IUiService _ui;
        private readonly ExportService _exportService;

        public ExportController(IUiService ui, ExportService exportService)
        {
            _ui = ui;
            _exportService = exportService;
        }

        /// <summary>
        /// Экспорт данных теста в Excel
        /// </summary>
        [HttpGet("excel/{testId}")]
        public async Task<IActionResult> ExportToExcel(string testId)
        {
            try
            {
                var test = await _ui.GetTestByIdAsync(testId);
                if (test == null)
                    return NotFound($"Тест с ID '{testId}' не найден");

                var fileBytes = await _exportService.ExportToExcelAsync(testId, test.Name);
                if (fileBytes == null)
                    return NotFound("Нет данных для экспорта");

                string fileName = $"ABTest_{test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при экспорте: {ex.Message}");
            }
        }

        /// <summary>
        /// Экспорт данных теста в TXT
        /// </summary>
        [HttpGet("txt/{testId}")]
        public async Task<IActionResult> ExportToTxt(string testId)
        {
            try
            {
                var test = await _ui.GetTestByIdAsync(testId);
                if (test == null)
                    return NotFound($"Тест с ID '{testId}' не найден");

                var txtContent = await _exportService.ExportToTxtAsync(testId, test.Name);
                if (txtContent == null)
                    return NotFound("Нет данных для экспорта");

                string fileName = $"ABTest_{test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var fileBytes = Encoding.UTF8.GetBytes(txtContent);
                return File(fileBytes, "text/plain", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при экспорте: {ex.Message}");
            }
        }

        /// <summary>
        /// Экспорт всех тестов в Excel
        /// </summary>
        [HttpGet("excel/all")]
        public async Task<IActionResult> ExportAllToExcel()
        {
            try
            {
                var fileBytes = await _exportService.ExportAllTestsToExcelAsync();
                if (fileBytes == null)
                    return NotFound("Нет данных для экспорта");

                string fileName = $"ABTests_All_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при экспорте: {ex.Message}");
            }
        }
    }
}