namespace Testing.DTO
{
    /// <summary>
    /// Общая аналитика системы A/B тестирования
    /// </summary>
    public class AnalyticDto
    {
        /// <summary> Всего тестов </summary>
        public int TotalTests { get; set; }

        /// <summary> Активных тестов </summary>
        public int ActiveTests { get; set; }

        /// <summary> Всего вариантов </summary>
        public int TotalVariants { get; set; }

        /// <summary> Всего пользователей </summary>
        public int TotalUsers { get; set; }

        /// <summary> Среднее число пользователей на тест </summary>
        public double AvgUsersPerTest { get; set; }

        /// <summary> Среднее число вариантов на тест </summary>
        public double AvgVariantsPerTest { get; set; }

        /// <summary> Среднее число результатов на тест </summary>
        public double AvgResultsPerTest { get; set; }

        /// <summary> Частота проведения тестов </summary>
        public double TestFrequency { get; set; }

        /// <summary> Конверсия системы </summary>
        public double AvgConversion { get; set; }

        /// <summary> Средний CTR </summary>
        public double AvgCtr { get; set; }

        /// <summary> Средний retention </summary>
        public double AvgRetention { get; set; }
    }
}