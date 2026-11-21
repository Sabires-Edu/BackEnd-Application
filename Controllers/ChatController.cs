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

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMeChat()
        {
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido..");

            var chats = await _chatService.GetMeChat(token, userid);

            return StatusCode(201, new SuccessResponse
            {
                Success = true,
                Title = "Retornado os chats do usuário corretamente.",
                Status = 201,
                Response = new Response
                {
                    Chats = chats
                }
            });
        }

        [Authorize]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChatById([FromRoute] int chatId, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de posts por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido.");

            var chat = await _chatService.GetChatById(token, userid, chatId, page, limit);
            var total = await _chatService.GetTotalChatMembers(token, chatId);

            return StatusCode(201, new SuccessResponse
            {
                Success = true,
                Title = "Retornado os detalhes do chat corretamente.",
                Status = 201,
                Response = new Response
                {
                    Chat = chat,
                    Pagination = new PaginationResponse
                    {
                        Page = page,
                        Limit = limit,
                        Total_Elements = total,
                        Total_Pages = (int)Math.Ceiling((double)total / limit)
                    }
                }
            });
        }

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
                Title = "Criado o chat corretamente.",
                Status = 201,
                Response = new Response
                {
                    Chat = chatResponse
                }
            });
        }

        [Authorize]
        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetChatMessages([FromRoute] int chatId, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de posts por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido.");

            var messages = await _chatService.GetChatMessages(token, userid, chatId, page, limit);
            var total = await _chatService.GetTotalChatMessages(token, chatId);

            return StatusCode(201, new SuccessResponse
            {
                Success = true,
                Title = "Retornado as mensagens do chat corretamente.",
                Status = 201,
                Response = new Response
                {
                    Messages = messages,
                    Pagination = new PaginationResponse
                    {
                        Page = page,
                        Limit = limit,
                        Total_Elements = total,
                        Total_Pages = (int)Math.Ceiling((double)total / limit)
                    }
                }
            });
        }
    }
}
