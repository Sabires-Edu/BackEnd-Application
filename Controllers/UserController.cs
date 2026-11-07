using Microsoft.AspNetCore.Mvc;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Utils.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Sabires_Backend_Application.Utils.Token;
using System.Security.Claims;

namespace Sabires_Backend_Application.Controllers
{
    [ApiController]
    [Route("/v1/users")]
    public class UserController(IUserService userService, TokenGenerator tokenGenerator) : ControllerBase
    {
        public readonly IUserService _userService = userService;
        public readonly TokenGenerator _tokenGenerator = tokenGenerator;

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

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

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

        [Authorize]
        [HttpGet("{userid}")]
        public async Task<IActionResult> GetUserById([FromRoute] string userid)
        {
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            var user = await _userService.GetUserById(userid, token);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Informações do usuário retornadas corretamente.",
                Status = 200,
                Response = new Response
                {
                    User = user
                }
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de usuários por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor faça login e tente novamente.");

            var users = await _userService.GetAllUsers(page, limit, token, userid);
            var total = await _userService.GetTotalUsers(token);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Lista de usuários retornada corretamente.",
                Status = 200,
                Response = new Response
                {
                    Users = users,
                    Pagination = new PaginationResponse
                    {
                        Page = page,
                        Limit = limit,
                        Total_Elements = total - 1,
                        Total_Pages = (int)Math.Ceiling((double)total / limit)
                    }
                },
            });
        }

        [Authorize]
        [HttpGet("followers/{userid}")]
        public async Task<IActionResult> GetFollowers([FromRoute] string userid, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de usuários por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            if (!Guid.TryParse(userid, out Guid useridparse))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor faça login e tente novamente.");

            var followers = await _userService.GetFollowers(page, limit, token, useridparse);
            var total = await _userService.GetTotalFollowerUsers(token, useridparse, "follower");

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Lista de seguidores retornada corretamente.",
                Status = 200,
                Response = new Response
                {
                    Users = followers,
                    Pagination = new PaginationResponse
                    {
                        Page = page,
                        Limit = limit,
                        Total_Elements = total,
                        Total_Pages = (int)Math.Ceiling((double)total / limit)
                    }
                },
            });
        }

        [Authorize]
        [HttpGet("me/followings")]
        public async Task<IActionResult> GetFollowings([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de usuários por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor faça login e tente novamente.");

            var followings = await _userService.GetFollowings(page, limit, token, userid);
            var total = await _userService.GetTotalFollowerUsers(token, userid, "following");

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Lista de seguidores retornada corretamente.",
                Status = 200,
                Response = new Response
                {
                    Users = followings,
                    Pagination = new PaginationResponse
                    {
                        Page = page,
                        Limit = limit,
                        Total_Elements = total,
                        Total_Pages = (int)Math.Ceiling((double)total / limit)
                    }
                },
            });
        }
    }
}
