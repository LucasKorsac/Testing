namespace Testing.Pattern
{
    /// <summary> Интерфейс стратегии расчёта веса варианта </summary>
    public interface IWeightStrategy
    {
        int CalculateWeight(int index, int count, int total, double average, double k = 1.0);
    }

    /// <summary> Стратегия расчёта веса варианта </summary>
    public class WeightStrategy : IWeightStrategy
    {
        /// <summary> Расчёт веса варианта </summary>
        public int CalculateWeight(int index, int count, int total, double average, double k = 1.0)
        {
            if (total <= 0) total = 1;
            if (count < 0) count = 0;

            // Формула: W = (i² × √count) / (k × n)
            double weight = (Math.Pow(index, 2) * Math.Sqrt(count)) / (k * total);

            return Math.Max(1, (int)Math.Round(weight));
        }
    }
}