using Microsoft.AspNetCore.Mvc;

namespace Sabires_Backend_Application_Controllers
{
    [ApiController]
    [Route("/v1/ping")]
    public class Health() : ControllerBase{
        public async Task<IActionResult> Ping(){
            return StatusCode(201, "Pong");
        }
    }
}