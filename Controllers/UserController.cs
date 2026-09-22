using Microsoft.AspNetCore.Mvc;

namespace Sabires_Backend_Application.Controllers
{
    public class UserController : ControllerBase
    {
        [HttpGet("ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok("Pong");
        }

    }
}
