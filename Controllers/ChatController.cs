using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Utils.Exceptions;
using Sabires_Backend_Application.Utils.Token;
using System.Security.Claims;

namespace Sabires_Backend_Application.Controllers
{
    [ApiController]
    [Route("/v1/chats")]

    public class ChatController(IChatService chatService, TokenGenerator tokenGenerator) : ControllerBase
    {
        public readonly IChatService _chatService = chatService;
        public readonly TokenGenerator _tokenGenerator = tokenGenerator;

        //[Authorize]
        //[HttpGet("me")]
        //public async Task<IActionResult> GetMeChat()
        //{            
        //    string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

        //    string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (!Guid.TryParse(useridClaim, out Guid userid))
        //        throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido..");

        //    var chats = await _chatService.GetMeChat();

        //    return StatusCode(201, new SuccessResponse
        //    {
        //        Success = true,
        //        Title = "Post criado corretamente.",
        //        Status = 201,
        //        Response = new Response
        //        {
        //            Chats = chats
        //        }
        //    });
        //}

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatDTO chat)
        {
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido..");

            var chatResponse = await _chatService.CreateChat(token, userid, chat);

            return StatusCode(201, new SuccessResponse
            {
                Success = true,
                Title = "Post criado corretamente.",
                Status = 201,
                Response = new Response
                {
                    Chat = chatResponse
                }
            });
        }

    }
}
