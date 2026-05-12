using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    public class TestWithVariants
    {
        public ABTests Test { get; set; }
        public List<Variants> Variants { get; set; } = new();
    }
    public class ApplicationWithInstances
    {
        public Applications Application { get; set; } = null!;

        public List<Instances> Instances { get; set; } = new();
    }
    public class InstanceMetricRow
    {
        public string User { get; set; } = "";

        public Dictionary<string, double> Metrics { get; set; } = new();
    }
}