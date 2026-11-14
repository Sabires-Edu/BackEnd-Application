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

        [HttpPost("me/follows/{followinguserid}")]
        public async Task<IActionResult> FollowUser([FromRoute] string followinguserid)
        {            
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            if (!Guid.TryParse(followinguserid, out Guid followinguseridParse))
                throw new UnauthorizedException("O identificador do usuário à ser seguido é inválido. Por favor faça login e tente novamente.");

            string followeruserid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(followeruserid, out Guid followeruseridParse))
                throw new UnauthorizedException("O identificador do usuário logado é inválido. Por favor faça login e tente novamente.");

            var response = await _userService.FollowUser(token, followeruseridParse, followinguseridParse);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Solicitação de amizade realizada corretamente.",
                Status = 200,
                Response = new Response
                {
                    FollowerId = response.ToString()
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
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido..");

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
        [HttpGet("{userid}/followers")]
        public async Task<IActionResult> GetFollowers([FromRoute] string userid, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de usuários por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            if (!Guid.TryParse(userid, out Guid useridparse))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido..");

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
        [HttpGet("{userid}/followings")]
        public async Task<IActionResult> GetFollowings([FromRoute] string userid, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de usuários por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            if (!Guid.TryParse(userid, out Guid useridparse))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido..");

            var followings = await _userService.GetFollowings(page, limit, token, useridparse);
            var total = await _userService.GetTotalFollowerUsers(token, useridparse, "following");

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

        [Authorize]
        [HttpGet("me/follows/pending")]
        public async Task<IActionResult> GetPendingFollows([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de usuários por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid useridparse))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido..");

            var follows = await _userService.GetPendingFollows(page, limit, token, useridparse);
            var total = await _userService.GetTotalPendingFollows(token, useridparse);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Lista de seguidores retornada corretamente.",
                Status = 200,
                Response = new Response
                {
                    Users = follows,
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
        [HttpPatch("me/follows/{followeruserid}")]
        public async Task<IActionResult> UpdateFollowStatus([FromRoute] string followeruserid, [FromBody] ChangeFollowStatusDTO status)
        {
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string followingUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(followingUserId, out Guid followingUserIdParse))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido.");

            if (!Guid.TryParse(followeruserid, out Guid followerUserIdParse))
                throw new UnauthorizedException("O usuário à ser alterado o status é inválido. Por favor insira um usuário válido.");

            await _userService.UpdateFollowStatus(token, followerUserIdParse, followingUserIdParse, status.Status);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Status do seguidor atualizado corretamente.",
                Status = 200,
                Response = new Response
                {
                    Result = $"Status do seguidor {(status.Status == 2 ? "aceito" : "recusado")} corretamente."
                }
            });
        }

        [Authorize]
        [HttpPatch("me")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO user)
        {
            if (user.FirstName == null && user.LastName == null && user.BirthDate == null && user.StatusId == null && user.Biography == null)
                throw new BadRequestException("Nenhum campo para atualização foi fornecido. Por favor insira pelo menos um campo para atualização.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido..");

            var student = await _userService.UpdateUser(token, user, userid);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Informações do usuário atualizadas corretamente.",
                Status = 200,
                Response = new Response
                {
                    Student = student
                }
            });
        }

        [Authorize]
        [HttpPatch("me/password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDTO password)
        {
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido..");

            await _userService.UpdatePassword(token, password, userid);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Senha do usuário atualizada corretamente.",
                Status = 200,
                Response = new Response
                {
                    Result = "Senha atualizada corretamente."
                }
            });
        }

        [Authorize]
        [HttpPut("me/picture")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile file)
        {
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido..");

            var fileUrl = await _userService.UploadProfilePicture(token, userid, file);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Foto de perfil atualizada corretamente.",
                Status = 200,
                Response = new Response
                {
                    Result = "Foto de perfil atualizada corretamente. Endereço: " + fileUrl
                }
            });
        }
    }
}
