using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    public class TestWithVariants
    {
        public ABTests Test { get; set; }
        public List<Variants> Variants { get; set; } = new();
    }
}