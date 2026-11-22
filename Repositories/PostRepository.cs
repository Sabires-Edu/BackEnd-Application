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
                    ParentPostId = postCreateDTO.ParentPostId ?? (postCreateDTO.RootPostId ?? 0),
                    LikesCount = 0,
                    CommentsCount = 0,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                };

                var response = await client.Rpc("create_post", post);

                if (string.IsNullOrEmpty(response.Content))
                    throw new InternalServerException("Não foi possível criar o novo post. Por favor, tente novamente mais tarde.");

                var postid = JsonSerializer.Deserialize<int>(response.Content);



                if(post.ParentPostId != 0)
                {
                    var parentPost = await GetPostById(token, post.ParentPostId);
                    var responseComment = await adminClient.From<Post>().Set(post => post.CommentsCount, parentPost.CommentsCount + 1).Where(pt => pt.PostId == parentPost.PostId).Update();

                    if(post.ParentPostId != post.RootPostId)
                    {
                        var rootPost = await GetPostById(token, post.RootPostId);
                        var responseRootComment = await adminClient.From<Post>().Set(post => post.CommentsCount, rootPost.CommentsCount + 1).Where(pt => pt.PostId == rootPost.PostId).Update();
                    }
                }                

                return postid;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new InternalServerException("Não foi possível criar o novo post. Por favor, tente novamente mais tarde.");
            }
        }22

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

                var response = await client.From<Post>().Order("createdat", Supabase.Postgrest.Constants.Ordering.Descending).Range((page - 1) * limit, (page * limit) - 1).Where(post => post.ParentPostId == null && post.DeletedAt == null).Get();

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
                Console.WriteLine(e.Message);
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

                    if (postResponse.RootPostId != postResponse.ParentPostId)
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

        public async Task<int> GetTotalComments(string token, int id)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await client.From<Post>().Where(post => post.ParentPostId == id).Count(CountType.Exact);

                return response;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar a quantidade de comentários do post. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<List<PostResponse>> GetUserPosts(string token, Guid userid, int page, int limit)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await client.From<Post>().Where(post => post.UserId == userid && post.DeletedAt == null).Range((page - 1) * limit, (page * limit) - 1).Order("createdat", Ordering.Descending).Get();

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
                throw new InternalServerException("Não foi possível buscar os posts do usuário. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetTotalUserPosts(string token, Guid userid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await client.From<Post>().Where(post => post.UserId == userid).Count(CountType.Exact);

                return response;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar a quantidade de posts do usuário. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<List<PostResponse>> GetUserLikes(string token, Guid userid, int page, int limit)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await client.From<PostLike>().Where(postLike => postLike.UserId == userid).Order("likedat", Ordering.Descending).Range((page - 1) * limit, (page * limit) - 1).Get();

                var posts = new List<PostResponse>();

                foreach (var post in response.Models)
                {
                    var fileList = new List<string>();

                    if (post.Post.Attachments != null)
                    {
                        foreach (var attachment in post.Post.Attachments)
                        {
                            fileList.Add(attachment.Filepath);
                        }
                    }

                    posts.Add(new PostResponse
                    {
                        PostId = post.Post.PostId,
                        UserId = post.Post.UserId,
                        Content = post.Post.Content,
                        RootPostId = post.Post.RootPostId ?? 0,
                        ParentPostId = post.Post.ParentPostId ?? 0,
                        Files = fileList,
                        LikesCount = post.Post.LikesCount,
                        CommentsCount = post.Post.CommentsCount,
                        CreatedAt = post.Post.CreatedAt,
                        UpdatedAt = post.Post.UpdatedAt,
                    });
                }

                return posts;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar os posts curtidos pelo usuário. Por favor, tente novamente mais tarde.");
            }

        }

        public async Task<int> GetTotalUserLikes(string token, Guid userid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await client.From<PostLike>().Where(postLike => postLike.UserId == userid).Count(CountType.Exact);

                return response;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar a quantidade de posts curtidos pelo usuário. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<List<PostResponse>> GetTrashPosts(string token, Guid userid, int page, int limit)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await client.From<Trash>().Where(trash => trash.UserId == userid).Order("deletedat", Ordering.Descending).Range((page - 1) * limit, (page * limit) - 1).Get();

                var posts = new List<PostResponse>();

                foreach (var post in response.Models)
                {
                    var fileList = new List<string>();

                    if (post.Post.Attachments != null)
                    {
                        foreach (var attachment in post.Post.Attachments)
                        {
                            fileList.Add(attachment.Filepath);
                        }
                    }

                    posts.Add(new PostResponse
                    {
                        PostId = post.Post.PostId,
                        UserId = post.Post.UserId,
                        Content = post.Post.Content,
                        RootPostId = post.Post.RootPostId ?? 0,
                        ParentPostId = post.Post.ParentPostId ?? 0,
                        Files = fileList,
                        LikesCount = post.Post.LikesCount,
                        CommentsCount = post.Post.CommentsCount,
                        CreatedAt = post.Post.CreatedAt,
                        UpdatedAt = post.Post.UpdatedAt,
                    });
                }

                return posts;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar os posts da lixeira do usuário. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetTotalTrashPosts(string token, Guid userid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);
                var response = await client.From<Trash>().Where(trash => trash.UserId == userid).Count(CountType.Exact);
                return response;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar a quantidade de posts da lixeira do usuário. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task LikePost(string token, Guid userid, int postid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var postResponse = await GetPostById(token, postid);

                PostLike postLike = new PostLike
                {
                    UserId = userid,
                    PostId = postid,
                    LikedAt = DateTimeOffset.UtcNow,
                };

                var response = await client.From<PostLike>().Insert(postLike);
                var updateResponse = await adminClient.From<Post>().Where(post => post.PostId == postid).Set(post => post.LikesCount, postResponse.LikesCount + 1).Update();
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível curtir o post. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task DislikePost(string token, Guid userid, int postid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var postResponse = await GetPostById(token, postid);

                var checkResponse = await client.From<PostLike>().Where(postLike => postLike.UserId == userid && postLike.PostId == postid).Count(CountType.Exact);

                if (checkResponse == 0)
                    throw new BadRequestException("O usuário logado não curtiu essa postagem, logo não é possível dar dislike");

                var response = await client.From<PostLike>().Where(postLike => postLike.UserId == userid && postLike.PostId == postid).Delete();
                var updateResponse = await adminClient.From<Post>().Where(post => post.PostId == postid).Set(post => post.LikesCount, postResponse.LikesCount - 1).Update();
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível descurtir o post. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task TrashPost(string token, Guid userid, int postid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var checkResponse = await client.From<Post>().Where(post => post.UserId == userid && post.PostId == postid).Count(CountType.Exact);

                if (checkResponse == 0)
                    throw new BadRequestException("O usuário logado não é o dono dessa postagem, logo não é possível mover para a lixeira.");

                var updateResponse = await client.From<Post>().Where(post => post.PostId == postid).Set(post => post.DeletedAt, DateTimeOffset.UtcNow).Update();

                var post = updateResponse.Models.FirstOrDefault();

                if(post.ParentPostId != null)
                {
                    var parentPost = await GetPostById(token, post.ParentPostId ?? 0);
                    var commentResponse = await client.From<Post>().Set(pt => pt.CommentsCount, parentPost.CommentsCount - 1).Where(pt => pt.PostId == parentPost.PostId).Update();

                    if (post.ParentPostId != post.RootPostId)
                    {
                        var rootPost = await GetPostById(token, post.RootPostId ?? 0);
                        var responseRootComment = await adminClient.From<Post>().Set(post => post.CommentsCount, rootPost.CommentsCount - 1).Where(pt => pt.PostId == rootPost.PostId).Update();
                    }
                }                

                Trash trash = new Trash
                {
                    UserId = userid,
                    PostId = postid,
                    DeletedAt = DateTimeOffset.UtcNow,
                };

                var response = await client.From<Trash>().Insert(trash);
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível mover o post para a lixeira. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task RestorePost(string token, Guid userid, int postid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var checkResponse = await client.From<Trash>().Where(trash => trash.UserId == userid && trash.PostId == postid).Count(CountType.Exact);

                if (checkResponse == 0)
                    throw new BadRequestException("O postagem não está na lixeira ou o usuário logado não é o dono dessa postagem, logo não é possível restaurar o post da lixeira.");

                var updateResponse = await client.From<Post>().Where(post => post.PostId == postid).Set(post => post.DeletedAt, null).Update();

                var post = updateResponse.Models.FirstOrDefault();

                if (post.ParentPostId != null)
                {
                    var parentPost = await GetPostById(token, post.ParentPostId ?? 0);
                    var commentResponse = await client.From<Post>().Set(pt => pt.CommentsCount, parentPost.CommentsCount + 1).Where(pt => pt.PostId == parentPost.PostId).Update();

                    if (post.ParentPostId != post.RootPostId)
                    {
                        var rootPost = await GetPostById(token, post.RootPostId ?? 0);
                        var responseRootComment = await adminClient.From<Post>().Set(post => post.CommentsCount, rootPost.CommentsCount + 1).Where(pt => pt.PostId == rootPost.PostId).Update();
                    }
                }

                var response = await client.From<Trash>().Where(trash => trash.UserId == userid && trash.PostId == postid).Delete();
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível restaurar o post da lixeira. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task RemovePost(string token, Guid userid, int postid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var checkResponse = await client.From<Trash>().Where(trash => trash.UserId == userid && trash.PostId == postid).Count(CountType.Exact);

                if (checkResponse == 0)
                    throw new BadRequestException("O postagem não está na lixeira ou o usuário logado não é o dono dessa postagem, logo não é possível remover o post da lixeira.");

                var response = await client.From<Post>().Where(post => post.PostId == postid).Delete();                
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível remover o post da lixeira. Por favor, tente novamente mais tarde.");
            }
        }
    }
}
