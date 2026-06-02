namespace Testing.DTO
{

    public class MetricDto
    {
        public string Id { get; set; } = "";
        public string ApplicationId { get; set; } = "";
        public string MetricTypeId { get; set; } = "";

        public double Meaning { get; set; }
    }
}