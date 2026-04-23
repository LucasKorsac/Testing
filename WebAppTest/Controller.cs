using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebAppTest;

namespace WebAppTest
{
    /// <summary>
    /// REST API контроллер для работы с A/B тестами, отвечает за HTTP-взаимодействие
    /// </summary>
    [ApiController] // Валидация моделей и поведение API
    [Route("api/ab")] // Маршрут
    public class Controller : ControllerBase
    {
        /// <summary>
        /// Сервис бизнес-логики
        /// </summary>
        private readonly ServiceControl _service;

        /// <summary>
        /// Конструктор с внедрением зависимости
        /// </summary>
        public Controller(ServiceControl service)
        {
            _service = service;
        }

        /// <summary>
        /// Запуск A/B тестирования. GET: api,ab,run
        /// </summary>
        [HttpGet("run")]
        public async Task<IActionResult> Run()
        {
            // Запуск бизнес-логики
            var result = await _service.Run(ObjectId.Empty);

            // Возврат результата клиенту
            return Ok(result);
        }
    }
}