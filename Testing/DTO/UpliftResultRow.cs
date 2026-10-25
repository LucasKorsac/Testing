using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.DTO
{
    public class UpliftResultRow
    {
        public string Userid { get; set; } = "";

        public int Treatment { get; set; }

        public string? Version { get; set; }

        public double Sum_gamerounds { get; set; }

        public int Retention_1 { get; set; }

        public int Retention_7 { get; set; }

        public double Uplift { get; set; }

        public double Retention_7_cuped { get; set; }
    }
}
