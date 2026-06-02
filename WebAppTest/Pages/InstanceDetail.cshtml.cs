using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Testing.DTO;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class InstanceDetail : PageModel
    {
        private readonly IUiService _ui;

        public InstanceDetail(IUiService ui)
        {
            _ui = ui;
        }

        public InstanceDto? Instance { get; set; }

        public List<TableRow> Table { get; set; } = new();

        public class TableRow
        {
            public string InstallId { get; set; } = "";

            public int VariantsCount { get; set; }

            public string VariantName { get; set; } = "";

            public string Type { get; set; } = "";

            public string Value { get; set; } = "";

            public double NumericValue { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToPage("/Apps");
            }

            Instance = await _ui.GetInstanceAsync(id);

            if (Instance == null)
            {
                return RedirectToPage("/Apps");
            }

            var results =
                await _ui.GetResultsByInstanceAsync(id);

            var metrics =
                await _ui.GetMetricsWithTypesAsync(
                    Instance.ApplicationId);

            var variants =
                await _ui.GetAllVariantsAsync();

            foreach (var result in results)
            {
                var variant =
                    variants.FirstOrDefault(v =>
                        v.Id == result.VariantId);

                if (variant == null)
                {
                    continue;
                }

                foreach (var metric in metrics)
                {
                    Table.Add(new TableRow
                    {
                        InstallId = result.InstanceId,

                        VariantsCount =
                            variants.Count(v =>
                                v.AbTestId ==
                                variant.AbTestId),

                        VariantName =
                            variant.Name,

                        Type =
                            metric.TypeName ??
                            "Метрика",

                        Value =
                            metric.Metric.Meaning
                                .ToString("0.00"),

                        NumericValue =
                            metric.Metric.Meaning
                    });
                }
            }

            return Page();
        }
    }
}