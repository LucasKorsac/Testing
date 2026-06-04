using ABLibrary.Models;
using Microsoft.AspNetCore.Mvc;

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
}
