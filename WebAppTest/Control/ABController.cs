using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace WebAppTest.Control
{
    /// <summary>
    /// Контроллер REST API для управления A/B тестированием.
    /// </summary>
    [ApiController] // Включает автоматическую валидацию моделей и поведение API
    [Route("api/ab")] // Базовый маршрут для всех методов контроллера
    public class ABController : ControllerBase
    {
        /// <summary>
        /// Сервис бизнес-логики A/B тестирования
        /// </summary>
        private readonly ServiceControl _service;

        /// <summary>
        /// Внедрение зависимостей через конструктор
        /// </summary>
        public ABController(ServiceControl service)
        {
            _service = service;
        }

        /// <summary>
        /// Модель запроса для запуска теста
        /// </summary>
        public class RunRequest
        {
            /// <summary>
            /// ID приложения
            /// </summary>
            public string AppId { get; set; }
        }

        /// <summary>
        /// Запуск A/B тестирования для указанного приложения
        /// </summary>
        [HttpPost("run")]
        public async Task<IActionResult> Run([FromBody] RunRequest request)
        {
            var objectId = ObjectId.Parse(request.AppId);

            var result = await _service.Run(objectId);

            return Ok(result);
        }

        /// <summary>
        /// Модель запроса для фиксации события конверсии
        /// </summary>
        public class ConvertRequest
        {
            /// <summary>
            /// Название теста
            /// </summary>
            public string TestName { get; set; }

            /// <summary>
            /// Выбранный вариант
            /// </summary>
            public string VariantName { get; set; }

            /// <summary>
            /// Идентификатор пользователя
            /// </summary>
            public string UserId { get; set; }
        }

        /// <summary>
        /// Фиксация события конверсии пользователя в тесте
        /// </summary>
        [HttpPost("convert")]
        public async Task<IActionResult> Convert([FromBody] ConvertRequest req)
        {
            await _service.Convert(req.TestName, req.VariantName, req.UserId);
            return Ok();
        }

        /// <summary>
        /// Получение статистики по тесту
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> Stats(string testName)
        {
            var events = await _service.GetEvents(testName);

            var result = events
                .GroupBy(e => e.VariantName)
                .Select(g =>
                {
                    // Количество показов варианта
                    var shows = g.Count(x => x.EventType == "show");

                    // Количество конверсий варианта
                    var conversions = g.Count(x => x.EventType == "conversion");

                    // Конверсия
                    return new
                    {variant = g.Key, shows, conversions, conversionRate = shows == 0
                            ? 0
                            : conversions / (double)shows
                    };
                });

            return Ok(result);
        }
    }
}