using Microsoft.AspNetCore.Mvc;
using Sabires_Backend_Application.Services.Interface;

namespace Sabires_Backend_Application.Controllers
{
    [ApiController]
    [Route("/v1/users/[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        public readonly IUserService _userService = userService;
            
        [HttpPost]
        public async Task<IActionResult> CreateUser()
        {
            var result = await _userService.CreateUser();

            return Ok(result);
        }

    }
}
