using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Testing.DTO;

namespace WebAppTest.Control
{
    /// <summary>
    /// Сервис для экспорта данных в различные форматы
    /// </summary>
    public class ExportService
    {
        private readonly IUiService _ui;

        public ExportService(IUiService ui)
        {
            _ui = ui;
        }

        /// <summary>
        /// Экспорт данных теста в Excel файл
        /// </summary>
        public async Task<byte[]> ExportToExcelAsync(string testId, string testName)
        {
            var test = await _ui.GetTestByIdAsync(testId);
            if (test == null) return null;

            var variants = await _ui.GetAllVariantsAsync();
            var testVariants = variants.Where(v => v.AbTestId == testId).ToList();
            var results = await _ui.GetResultsAsync(testId);
            var metrics = await _ui.GetMetricsWithTypesAsync(test.ApplicationId);

            using (var package = new ExcelPackage())
            {
                // Лист с общей информацией
                var summarySheet = package.Workbook.Worksheets.Add("Общая информация");
                FillSummarySheet(summarySheet, test, testVariants, results);

                // Лист с вариантами
                var variantsSheet = package.Workbook.Worksheets.Add("Варианты");
                FillVariantsSheet(variantsSheet, testVariants, results);

                // Лист с метриками
                var metricsSheet = package.Workbook.Worksheets.Add("Метрики");
                FillMetricsSheet(metricsSheet, metrics, results, testVariants);

                // Лист с детальными результатами
                var detailsSheet = package.Workbook.Worksheets.Add("Детальные результаты");
                await FillDetailsSheetAsync(detailsSheet, testId, test.ApplicationId);

                return package.GetAsByteArray();
            }
        }

        /// <summary>
        /// Экспорт данных теста в текстовый файл
        /// </summary>
        public async Task<string> ExportToTxtAsync(string testId, string testName)
        {
            var test = await _ui.GetTestByIdAsync(testId);
            if (test == null) return null;

            var variants = await _ui.GetAllVariantsAsync();
            var testVariants = variants.Where(v => v.AbTestId == testId).ToList();
            var results = await _ui.GetResultsAsync(testId);
            var metrics = await _ui.GetMetricsWithTypesAsync(test.ApplicationId);

            var sb = new StringBuilder();

            // Заголовок
            sb.AppendLine("=".PadRight(60, '='));
            sb.AppendLine($"ОТЧЕТ ПО A/B ТЕСТУ");
            sb.AppendLine("=".PadRight(60, '='));
            sb.AppendLine();
            sb.AppendLine($"Название теста: {test.Name}");
            sb.AppendLine($"Описание: {test.Description ?? "Нет описания"}");
            sb.AppendLine($"Статус: {(test.Enabled ? "Активен" : "Остановлен")}");
            sb.AppendLine($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            sb.AppendLine();

            // Статистика
            sb.AppendLine("-".PadRight(60, '-'));
            sb.AppendLine("ОБЩАЯ СТАТИСТИКА");
            sb.AppendLine("-".PadRight(60, '-'));
            sb.AppendLine($"Количество вариантов: {testVariants.Count}");
            sb.AppendLine($"Количество установок: {results.Count}");
            sb.AppendLine();

            // Варианты
            sb.AppendLine("-".PadRight(60, '-'));
            sb.AppendLine("ИНФОРМАЦИЯ ПО ВАРИАНТАМ");
            sb.AppendLine("-".PadRight(60, '-'));
            foreach (var variant in testVariants)
            {
                var variantResults = results.Count(r => r.VariantId == variant.Id);
                sb.AppendLine($"  Вариант: {variant.Name}");
                sb.AppendLine($"    Описание: {variant.Description ?? "Нет описания"}");
                sb.AppendLine($"    Установок: {variantResults}");
                sb.AppendLine($"    Процент: {(results.Count > 0 ? (variantResults * 100.0 / results.Count).ToString("F2") : "0")}%");
                sb.AppendLine();
            }

            // Метрики
            sb.AppendLine("-".PadRight(60, '-'));
            sb.AppendLine("МЕТРИКИ");
            sb.AppendLine("-".PadRight(60, '-'));
            foreach (var metric in metrics)
            {
                sb.AppendLine($"  {metric.TypeName ?? "Метрика"}: {metric.Metric.Meaning:F2}");
            }
            sb.AppendLine();

            // Подвал
            sb.AppendLine("=".PadRight(60, '='));
            sb.AppendLine("Конец отчета");
            sb.AppendLine("=".PadRight(60, '='));

            return sb.ToString();
        }

        /// <summary>
        /// Экспорт всех тестов в Excel
        /// </summary>
        public async Task<byte[]> ExportAllTestsToExcelAsync()
        {
            var tests = await _ui.GetTestsAsync();
            var variants = await _ui.GetAllVariantsAsync();

            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Все тесты");

                // Заголовки
                sheet.Cells[1, 1].Value = "ID";
                sheet.Cells[1, 2].Value = "Название";
                sheet.Cells[1, 3].Value = "Описание";
                sheet.Cells[1, 4].Value = "Статус";
                sheet.Cells[1, 5].Value = "Количество вариантов";
                sheet.Cells[1, 6].Value = "Дата создания";

                using (var range = sheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Данные
                int row = 2;
                foreach (var test in tests)
                {
                    var testVariants = variants.Count(v => v.AbTestId == test.Id);
                    sheet.Cells[row, 1].Value = test.Id;
                    sheet.Cells[row, 2].Value = test.Name;
                    sheet.Cells[row, 3].Value = test.Description;
                    sheet.Cells[row, 4].Value = test.Enabled ? "Активен" : "Остановлен";
                    sheet.Cells[row, 5].Value = testVariants;
                    sheet.Cells[row, 6].Value = DateTime.Now.ToString("dd.MM.yyyy");
                    row++;
                }

                sheet.Cells.AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        #region Private Methods

        private void FillSummarySheet(ExcelWorksheet sheet, TestDto test, List<VariantDto> variants, List<AbResultsDto> results)
        {
            sheet.Cells[1, 1].Value = "Параметр";
            sheet.Cells[1, 2].Value = "Значение";
            sheet.Cells[1, 1, 1, 2].Style.Font.Bold = true;

            sheet.Cells[2, 1].Value = "Название теста";
            sheet.Cells[2, 2].Value = test.Name;
            sheet.Cells[3, 1].Value = "Описание";
            sheet.Cells[3, 2].Value = test.Description ?? "Нет описания";
            sheet.Cells[4, 1].Value = "Статус";
            sheet.Cells[4, 2].Value = test.Enabled ? "Активен" : "Остановлен";
            sheet.Cells[5, 1].Value = "Количество вариантов";
            sheet.Cells[5, 2].Value = variants.Count;
            sheet.Cells[6, 1].Value = "Количество установок";
            sheet.Cells[6, 2].Value = results.Count;
            sheet.Cells[7, 1].Value = "Дата формирования";
            sheet.Cells[7, 2].Value = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            sheet.Cells.AutoFitColumns();
        }

        private void FillVariantsSheet(ExcelWorksheet sheet, List<VariantDto> variants, List<AbResultsDto> results)
        {
            sheet.Cells[1, 1].Value = "Название";
            sheet.Cells[1, 2].Value = "Описание";
            sheet.Cells[1, 3].Value = "Установки";
            sheet.Cells[1, 4].Value = "Процент";
            sheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;

            int row = 2;
            foreach (var variant in variants)
            {
                var variantResults = results.Count(r => r.VariantId == variant.Id);
                var percent = results.Count > 0 ? (variantResults * 100.0 / results.Count) : 0;

                sheet.Cells[row, 1].Value = variant.Name;
                sheet.Cells[row, 2].Value = variant.Description ?? "Нет описания";
                sheet.Cells[row, 3].Value = variantResults;
                sheet.Cells[row, 4].Value = $"{percent:F2}%";
                row++;
            }

            sheet.Cells.AutoFitColumns();
        }

        private void FillMetricsSheet(ExcelWorksheet sheet, List<MetricWithTypeDto> metrics, List<AbResultsDto> results, List<VariantDto> variants)
        {
            sheet.Cells[1, 1].Value = "Метрика";
            sheet.Cells[1, 2].Value = "Значение";
            sheet.Cells[1, 1, 1, 2].Style.Font.Bold = true;

            int row = 2;
            foreach (var metric in metrics)
            {
                sheet.Cells[row, 1].Value = metric.TypeName ?? "Метрика";
                sheet.Cells[row, 2].Value = metric.Metric.Meaning;
                row++;
            }

            sheet.Cells.AutoFitColumns();
        }

        private async Task FillDetailsSheetAsync(ExcelWorksheet sheet, string testId, string applicationId)
        {
            var results = await _ui.GetResultsAsync(testId);
            var variants = await _ui.GetAllVariantsAsync();
            var metrics = await _ui.GetMetricsWithTypesAsync(applicationId);
            var testVariants = variants.Where(v => v.AbTestId == testId).ToDictionary(v => v.Id, v => v.Name);

            sheet.Cells[1, 1].Value = "Установка";
            sheet.Cells[1, 2].Value = "Вариант";
            sheet.Cells[1, 3].Value = "Метрика";
            sheet.Cells[1, 4].Value = "Значение";
            sheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;

            int row = 2;
            foreach (var result in results)
            {
                var variantName = testVariants.GetValueOrDefault(result.VariantId, "Неизвестно");

                foreach (var metric in metrics)
                {
                    sheet.Cells[row, 1].Value = result.InstanceId;
                    sheet.Cells[row, 2].Value = variantName;
                    sheet.Cells[row, 3].Value = metric.TypeName ?? "Метрика";
                    sheet.Cells[row, 4].Value = metric.Metric.Meaning;
                    row++;
                }
            }

            sheet.Cells.AutoFitColumns();
        }

        #endregion
    }
}