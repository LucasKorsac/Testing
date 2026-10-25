using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.DTO
{
    public class UpliftMetrics
    {
        public int Users { get; set; }

        public int ControlUsers { get; set; }

        public int TreatmentUsers { get; set; }

        public double ControlMean { get; set; }

        public double TreatmentMean { get; set; }

        public double ObservedUplift { get; set; }

        public double PredictedUpliftMean { get; set; }

        public double PredictedUpliftMedian { get; set; }

        public int PositiveUpliftUsers { get; set; }

        public int NegativeUpliftUsers { get; set; }

        public string? TestId { get; set; }

        public string? Model { get; set; }

        public bool Cuped { get; set; }
    }
}
