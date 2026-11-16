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
    [Route("/v1/posts")]
    public class PostController(IPostService postService, TokenGenerator tokenGenerator) : ControllerBase
    {
        public readonly IPostService _postService = postService;
        public readonly TokenGenerator _tokenGenerator = tokenGenerator;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] PostCreateDTO postCreateDTO)
        {
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido..");

            var post = await _postService.CreatePost(token, userid, postCreateDTO);

            return StatusCode(201, new SuccessResponse
            {
                Success = true,
                Title = "Post criado corretamente.",
                Status = 201,
                Response = new Response
                {
                    Result = $"Post criado com sucesso. PostId {post}"
                }
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllPosts([FromQuery] int page = 1, int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de posts por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido.");

            var posts = await _postService.GetAllPosts(page, limit, token, userid);
            var total = await _postService.GetTotalPosts(token, userid);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Lista de posts retornada corretamente.",
                Status = 200,
                Response = new Response
                {
                    Posts = posts,
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
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            var post = await _postService.GetPostById(token, id);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Post retornado corretamente.",
                Status = 201,
                Response = new Response
                {
                    Post = post
                }
            });
        }
    }
}
