using System.Collections.Generic;
using Testing.DTO;

namespace Testing.Pattern
{
    /// <summary>
    /// Интерфейс для MAB стратегий (расширяет IStrategy)
    /// </summary>
    public interface IMABStrategy : IStrategy<VariantDto>
    {
        /// <summary>
        /// Обновление статистики после получения нового результата
        /// </summary>
        void UpdateStats(string variantId, double reward);

        /// <summary>
        /// Сброс статистики (для нового теста)
        /// </summary>
        void ResetStats();

        /// <summary>
        /// Получение текущих статистик по вариантам
        /// </summary>
        Dictionary<string, VariantStats> GetCurrentStats();
    }
}