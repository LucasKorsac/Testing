//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using ABLibrary.Models;

//namespace Testing.Control
//{
//    [ApiController]
//    [Route("api/ab")]
//    public class ABController : ControlBase
//    {
//        [HttpGet("config/{appId}")]
//        public ActionResult<ServerConfig> GetConfig(
//            string appId)
//        {
//            var config = new ServerConfig();

//            /* Здесь потом должно быть MongoDB  */

//            var tests =
//                Controller.I.CurrentTests;

//            foreach (var t in tests)
//            {
//                config.Tests[t.Key] =
//                    t.Value.ToString();
//            }

//            return Ok(config);
//        }

//        [HttpPost("event")]
//        public IActionResult Event(
//            [FromBody] TestEvent evt)
//        {
//            Console.WriteLine(
//                $"USER={evt.UserId} " +
//                $"TEST={evt.TestName} " +
//                $"VARIANT={evt.Variant}");

//            return Ok();
//        }
//    }
