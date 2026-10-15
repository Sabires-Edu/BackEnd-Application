using Microsoft.AspNetCore.Mvc;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Utils.Exceptions;

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

        [HttpPost("me/login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO user)
        {
            var token = await _userService.Login(user);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Usuário logado corretamente.",
                Status = 200,
                Response = new Response
                {
                    JWT = token
                }
            });
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            string auth = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(auth) || !auth.StartsWith("Bearer "))
                throw new UnauthorizedException("Token de autenticação ausente ou inválido.");

            string token = auth.Replace("Bearer ", "");

            var user = await _userService.Me(token);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Informações do usuário autenticado retornadas corretamente.",
                Status = 200,
                Response = new Response
                {
                    Me = user
                }
            });
        }
    }
}
