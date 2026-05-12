using Microsoft.AspNetCore.Mvc;
using Testing.Base;
using Testing.Pattern;
using WebAppTest.Control;

namespace WebAppTest.Controllers
{
    [ApiController]
    [Route("api/ab")]
    public class AbController : ControllerBase
    {
        private readonly IUiService _ui;

        public AbController(IUiService ui)
        {
            _ui = ui;
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run([FromBody] RunRequest request)
        {
            var result = await _ui.GetActiveTestsAsync(request.AppId);
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
    }

    public record RunRequest(string AppId);
    public record IdRequest(string Id);
    public record UpdateRequest(string Id, string Name, string Description);
}