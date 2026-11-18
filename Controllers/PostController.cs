using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sabires_Backend_Application.Models;
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
        [HttpGet("{id}")]
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

        [Authorize]
        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments(int id, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de posts por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            var posts = await _postService.GetComments(token, id, page, limit);
            var total = await _postService.GetTotalComments(token, id);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Comentários retornado corretamente.",
                Status = 201,
                Response = new Response
                {
                    RootPost = posts.RootPost,
                    ParentPost = posts.ParentPost,
                    Post = posts.Post,
                    Posts = posts.Comments,
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
        [HttpGet("me")]
        public async Task<IActionResult> GetUserPosts([FromQuery] int page = 1, int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de posts por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido.");

            var posts = await _postService.GetUserPosts(token, userid, page, limit);
            var total = await _postService.GetTotalUserPosts(token, userid);

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
        [HttpGet("me/likes")]
        public async Task<IActionResult> GetUserLikes([FromQuery] int page = 1, int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de posts por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido.");

            var posts = await _postService.GetUserLikes(token, userid, page, limit);
            var total = await _postService.GetTotalUserLikes(token, userid);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Lista de posts curtidos retornada corretamente.",
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
        [HttpGet("me/trash")]
        public async Task<IActionResult> GetTrashPosts([FromQuery] int page = 1, int limit = 10)
        {
            if (page < 1)
                throw new BadRequestException("Insira uma página válida.");

            if (limit < 1)
                throw new BadRequestException("Insira uma quantidade válida de posts por página.");

            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido.");

            var posts = await _postService.GetTrashPosts(token, userid, page, limit);
            var total = await _postService.GetTotalTrashPosts(token, userid);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Lista de posts na lixeira retornada corretamente.",
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
        [HttpPost("me/like/{id}")]
        public async Task<IActionResult> LikePost(int id)
        {
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido.");

            await _postService.LikePost(token, userid, id);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Post curtido corretamente.",
                Status = 200,
                Response = new Response
                {
                    Result = $"Post {id} curtido com sucesso."
                }
            });
        }

        [Authorize]
        [HttpDelete("me/dislike/{id}")]
        public async Task<IActionResult> DislikePost(int id)
        {
            string token = _tokenGenerator.CheckToken(Request.Headers["Authorization"]);

            string useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(useridClaim, out Guid userid))
                throw new UnauthorizedException("O identificador do usuário é inválido. Por favor insira um usuário válido.");

            await _postService.DislikePost(token, userid, id);

            return StatusCode(200, new SuccessResponse
            {
                Success = true,
                Title = "Post descurtido corretamente.",
                Status = 200,
                Response = new Response
                {
                    Result = $"Post {id} descurtido com sucesso."
                }
            });
        }
    }
}
