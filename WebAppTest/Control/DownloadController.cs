using Microsoft.AspNetCore.Mvc;

namespace WebAppTest.Control
{
    [ApiController]
    [Route("download")]
    public class DownloadController : ControllerBase
    {
        [HttpGet("ABLibrary.dll")]
        public IActionResult DownloadLibrary()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "download", "ABLibrary.dll");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Файл библиотеки не найден");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/x-msdownload", "ABLibrary.dll");
        }
    }
}
