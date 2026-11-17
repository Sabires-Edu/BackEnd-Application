using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Utils.Exceptions;
using Supabase.Gotrue;
using Supabase.Postgrest;
using System.Text.Json;
using static Supabase.Postgrest.Constants;

namespace Sabires_Backend_Application.Repositories
{
    public class PostRepository(SupabaseClientService Client) : IPostRepository
    {
        private readonly SupabaseClientService _client = Client;
        private readonly Supabase.Client adminClient = Client.AdminClient;

        public async Task<int> CreatePost(string token, Guid userid, PostCreateDTO postCreateDTO)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var post = new PostFunction
                {
                    UserId = userid,
                    Content = postCreateDTO.Content,
                    RootPostId = postCreateDTO.RootPostId ?? 0,
                    ParentPostId = postCreateDTO.ParentPostId ?? 0,
                    LikesCount = 0,
                    CommentsCount = 0,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                };

                var response = await client.Rpc("create_post", post);

                if (string.IsNullOrEmpty(response.Content))
                    throw new InternalServerException("Não foi possível criar o novo post. Por favor, tente novamente mais tarde.");

                var postid = JsonSerializer.Deserialize<int>(response.Content);

                return postid;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível criar o novo post. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<PostResponse> GetPostById(string token, int id)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await client.From<Post>().Where(post => post.PostId == id).Get();

                if (response.Models.Count == 0)
                    throw new BadRequestException("Post não encontrado.");

                var post = response.Models.FirstOrDefault();

                var fileList = new List<string>();

                if (post.Attachments != null)
                {
                    foreach (var attachment in post.Attachments)
                    {
                        fileList.Add(attachment.Filepath);
                    }
                }

                return new PostResponse
                {
                    PostId = post.PostId,
                    UserId = post.UserId,
                    Content = post.Content,
                    RootPostId = post.RootPostId ?? 0,
                    ParentPostId = post.ParentPostId ?? 0,
                    Files = fileList,
                    LikesCount = post.LikesCount,
                    CommentsCount = post.CommentsCount,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                };
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar o post. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task DeletePostById(string token, int id)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await client.From<Post>().Where(post => post.PostId == id).Delete();                
            }
            catch (Exception e)
            {                
                throw new InternalServerException("Não foi possível deletar o post. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task InsertFile(string token, int postid, Dictionary<string, string> files)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                foreach (var file in files)
                {
                    PostAttachment postAttachment = new PostAttachment
                    {
                        PostId = postid,
                        Filename = file.Key,
                        Filepath = file.Value,
                        Filetypeid = 1,
                        Uploadedat = DateTimeOffset.UtcNow,
                    };

                    var response = await client.From<PostAttachment>().Insert(postAttachment);
                }
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível inserir os arquivos do post. Por favor, tente novamente mais tarde.");
            }
        }
    
        public async Task<List<PostResponse>> GetAllPosts(int page, int limit, string token, Guid userid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await client.From<Post>().Order("createdat", Supabase.Postgrest.Constants.Ordering.Descending).Range((page - 1) * limit, (page * limit) - 1).Where(post => post.UserId != userid).Get();

                var posts = new List<PostResponse>();

                foreach (var post in response.Models)
                {
                    var fileList = new List<string>();

                    if (post.Attachments != null)
                    {
                        foreach (var attachment in post.Attachments)
                        {
                            fileList.Add(attachment.Filepath);
                        }
                    }

                    posts.Add(new PostResponse
                    {
                        PostId = post.PostId,
                        UserId = post.UserId,
                        Content = post.Content,
                        RootPostId = post.RootPostId ?? 0,
                        ParentPostId = post.ParentPostId ?? 0,
                        Files = fileList,
                        LikesCount = post.LikesCount,
                        CommentsCount = post.CommentsCount,
                        CreatedAt = post.CreatedAt,
                        UpdatedAt = post.UpdatedAt,
                    });
                }

                return posts;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar os posts. Por favor, tente novamente mais tarde.");
            }
        }
    
        public async Task<int> GetTotalPosts(string token, Guid userid)
        {
            try
            {                
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await client.From<Post>().Where(post => post.UserId != userid).Count(CountType.Exact);

                return response;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar a quantidade de posts. Por favor, tente novamente mais tarde.");
            }
        }
    
        public async Task<CommentResponse> GetComments(string token, int id, int page, int limit)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var postResponse = await GetPostById(token, id);

                PostResponse? rootPostResponse = null;
                PostResponse? parentPostResponse = null;

                if (postResponse.RootPostId != postResponse.PostId)
                {
                    rootPostResponse = await GetPostById(token, postResponse.RootPostId);

                    if(postResponse.RootPostId != postResponse.ParentPostId)
                        parentPostResponse = await GetPostById(token, postResponse.ParentPostId);
                }

                var response = await client.From<Post>().Where(post => post.ParentPostId == id).Range((page - 1) * limit, (page * limit) - 1).Order("likescount", Ordering.Descending).Get();

                var comments = new List<PostResponse>();

                foreach (var post in response.Models)
                {
                    var fileList = new List<string>();

                    if (post.Attachments != null)
                    {
                        foreach (var attachment in post.Attachments)
                        {
                            fileList.Add(attachment.Filepath);
                        }
                    }

                    comments.Add(new PostResponse
                    {
                        PostId = post.PostId,
                        UserId = post.UserId,
                        Content = post.Content,
                        RootPostId = post.RootPostId ?? 0,
                        ParentPostId = post.ParentPostId ?? 0,
                        Files = fileList,
                        LikesCount = post.LikesCount,
                        CommentsCount = post.CommentsCount,
                        CreatedAt = post.CreatedAt,
                        UpdatedAt = post.UpdatedAt,
                    });
                }

                return new CommentResponse
                {
                    Post = postResponse,
                    RootPost = rootPostResponse,
                    ParentPost = parentPostResponse,
                    Comments = comments
                };
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar os comentários do post. Por favor, tente novamente mais tarde.");
            }
        }
    }
}
