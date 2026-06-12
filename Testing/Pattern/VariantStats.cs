namespace Testing.Pattern
{
    /// <summary>
    /// Статистика варианта для MAB
    /// </summary>
    public class VariantStats
    {
        public string VariantId { get; set; } = "";
        public string VariantName { get; set; } = "";
        public int Successes { get; set; } = 0;
        public int Failures { get; set; } = 0;
        public double TotalReward { get; set; } = 0;
        public int Count { get; set; } = 0;

        public double ConversionRate => Count > 0 ? (double)Successes / Count : 0;
    }
}