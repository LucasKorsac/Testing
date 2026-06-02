namespace Testing.DTO
{
    public class MetricWithTypeDto
    {
        public MetricDto Metric { get; set; } = new();

        public string? TypeName { get; set; }
    }
}