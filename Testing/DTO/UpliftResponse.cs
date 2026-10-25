using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.DTO
{
    public class UpliftResponse
    {
        public bool Success { get; set; }

        public string? Error { get; set; }

        public UpliftMetrics? Metrics { get; set; }

        public List<UpliftResultRow> Rows { get; set; } =
            new();
    }
}
