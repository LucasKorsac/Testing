using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing
{
    internal class App
    {
        public async Task Init()
        {
            // Инициализация синтетических данных в MongoDB
            await SinteticData.Init();

            // Получение репозиториев для работы с A/B тестами и вариантами
            var abTestRepo = Repos.AbTest;
            var variantRepo = Repos.Variant;

            // Создание фасада, который упрощает работу с A/B тестами
            var facade = new Facade(Repos.AbTest, Repos.Variant);

            // Выбор стратегии: в данном случае случайный выбор варианта
            IStrategy<Variants> strategy = new RandomStrategy<Variants>();

            // сервис/пример, который запускает A/B тестирование
            var example = new Example(facade, strategy, variantRepo);

            // Попытка получить applicationId
            var applicationId = ObjectId.Parse;

            // Запуск логики A/B теста
            await example.Init();

            // Вывод результатов A/B теста в консоль
            foreach (var item in example.AB)
            {
                Console.WriteLine($"{item.Key} -> {item.Value}");
            }

            // Финальное сообщение о завершении теста
            Console.WriteLine("A/B test finished");
        }
    }
}