using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ABLibrary.Interfaces
{
    public interface IABManager
    {
        // Инициализация
        Task InitAsync(string gameId);

        // Получение варианта теста
        string GetVariant(string testName);

        // Отслеживание события
        Task TrackAsync(string testName, string userId, string eventType = "conversion");

        // Отправка накопленных событий
        Task FlushAsync();

        // Свойства
        bool IsInitialized { get; }
        string CurrentGameId { get; }
    }
}