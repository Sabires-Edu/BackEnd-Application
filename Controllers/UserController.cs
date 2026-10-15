using Microsoft.AspNetCore.Mvc;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Utils;

namespace Sabires_Backend_Application.Controllers
{
    [ApiController]
    [Route("/v1/users")]
    public class UserController(IUserService userService) : ControllerBase
    {
        public readonly IUserService _userService = userService;        

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO user)
        {        
            var token = await _userService.CreateUser(user);            

            return StatusCode(201, new SuccessResponse
            {
                Success = true,
                Title = "Usuário criado corretamente.",
                Status = 201,
                Response = new Response
                {
                    JWT = token
                }
            });
        }

    }
}
