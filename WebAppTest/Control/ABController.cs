using ABLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using WebAppTest.Controllers;

namespace WebAppTest.Control
{
    [ApiController]
    [Route("api/ab")]
    public class ABController : ControllerBase
    {
        private readonly IUiService _ui;

        public ABController(IUiService ui)
        {
            _ui = ui;
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run([FromBody] RunRequest request)
        {
            // Получаем instanceId из запроса
            var result = await _ui.GetActiveTestsAsync(request.AppId, request.InstanceId);
            return Ok(result);
        }

        [HttpPost("stop")]
        public async Task<IActionResult> Stop([FromBody] IdRequest request)
        {
            await _ui.StopTestAsync(request.Id);
            return Ok();
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] IdRequest request)
        {
            await _ui.DeleteTestAsync(request.Id);
            return Ok();
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateRequest request)
        {
            await _ui.UpdateTestAsync(request.Id, request.Name, request.Description);
            return Ok();
        }

        // GET api/ab/config/{appId}
        //[HttpGet("config/{appId}")]
        //public async Task<IActionResult> GetConfig(string appId)
        //{
        //    var tests =
        //        await _ui.GetActiveTestsAsync(appId);

        //    var config =
        //        new ServerConfig
        //        {
        //            Tests = tests.ToString()
        //        };

        //    return Ok(config);
        //}

        // POST api/ab/event
        [HttpPost("event")]
        public async Task<IActionResult> SendEvent(
            [FromBody] TestEvent evt)
        {
            if (evt == null)
            {
                return BadRequest();
            }

            // сохранить результат теста
            await _ui.SaveEventAsync(evt);

            return Ok();
        }
    }


        

    public record RunRequest(string AppId, string InstanceId = "");
    public record IdRequest(string Id);
    public record UpdateRequest(string Id, string Name, string Description);
}