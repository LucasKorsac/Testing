using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.DTO
{
    public class UpliftData
    {
        public string UserId { get; set; } = "";

        public string Version { get; set; } = "";

        public int Treatment { get; set; }

        public double SumGameRounds { get; set; }

        public int Retention1 { get; set; }

        public int Retention7 { get; set; }
    }
}
