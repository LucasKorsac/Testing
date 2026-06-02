namespace Testing.DTO.Charts
{
    /// <summary> Универсальный DTO для графиков </summary>
    public class ChartDto
    {
        public string Id { get; set; } = "";

        public string Type { get; set; } = "line";

        public string Title { get; set; } = "";

        public List<string> Labels { get; set; } = new();

        public List<double> Values { get; set; } = new();
    }
}