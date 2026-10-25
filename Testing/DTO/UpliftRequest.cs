using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.DTO
{
    public class UpliftRequest
    {
        public string TestId { get; set; } = "";

        // Названия колонок в Data
        public string UserIdColumn { get; set; } = "userid";
        public string TreatmentColumn { get; set; } = "treatment";
        public string TargetColumn { get; set; } = "target";

        // Признаки, измеренные до эксперимента
        public List<string> Features { get; set; } = new();

        // Необязательная колонка для CUPED
        public string? CupedColumn { get; set; }

        public string Model { get; set; } = "DR";
        public bool Cuped { get; set; } = false;
        public bool CrossValidate { get; set; } = true;

        // Произвольные столбцы и строки датасета
        public List<Dictionary<string, System.Text.Json.JsonElement>>
            Data
        { get; set; } = new();
    }
}
