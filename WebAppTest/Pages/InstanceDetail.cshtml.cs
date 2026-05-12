using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using WebAppTest.Control;
using static Testing.Base.BaseMongo;

namespace WebAppTest.Pages
{
    public class InstanceDetail : PageModel
    {
        private readonly IUiService _ui;

        public InstanceDetail(IUiService ui)
        {
            _ui = ui;
        }

        // =========================================
        // TABLE MODEL
        // =========================================

        public class TableRow
        {
            public string User { get; set; } = "";

            public Dictionary<string, string> Values { get; set; } = new();
        }

        // =========================================
        // DATA
        // =========================================

        public Instances? Instance { get; set; }

        public List<TableRow> Table { get; set; } = new();

        public List<string> Headers { get; set; } = new();

        // =========================================
        // PAGE
        // =========================================

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return RedirectToPage("/Apps");

            // экземпляр
            Instance = await _ui.GetInstanceAsync(id);

            if (Instance == null)
                return RedirectToPage("/Apps");

            // приложение
            var apps = await _ui.GetApplicationWithInstanceAsync();

            var app = apps
                .FirstOrDefault(x =>
                    x.Application.Id == Instance.ApplicationId);

            if (app == null)
                return Page();

            // метрики приложения
            var metrics = await _ui.GetMetricsWithTypesAsync(
                app.Application.Id.ToString());

            // строка таблицы
            var row = new TableRow
            {
                User = "User 1"
            };

            foreach (var item in metrics)
            {
                var metric = item.Metric;

                var type = item.Type;

                if (type == null)
                    continue;

                row.Values[type.Name] =
                    metric.Meaning.ToString("0.##");
            }

            Table.Add(row);

            // заголовки
            Headers = row.Values.Keys.ToList();

            return Page();
        }
    }
}