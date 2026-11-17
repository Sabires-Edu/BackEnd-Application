using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Utils.Exceptions;
using Sabires_Backend_Application.Utils.Validation;

namespace Sabires_Backend_Application.Services
{
    public class PostService(IPostRepository postRespository, IStorageSupabase storageSupabase) : IPostService
    {
        private readonly IPostRepository _postRepository = postRespository;
        private readonly IStorageSupabase _storageSupabase = storageSupabase;
        private readonly PostValidation postValidation = new PostValidation();

        public async Task<int> CreatePost(string token, Guid userid, PostCreateDTO postCreateDTO)
        {
            postValidation.ValidatePostContent(postCreateDTO.Content);
            postValidation.ValidatePostFiles(postCreateDTO.Images);

            if (postCreateDTO.RootPostId.HasValue)
                await _postRepository.GetPostById(token, postCreateDTO.RootPostId ?? 0);

            if (postCreateDTO.ParentPostId.HasValue)
                await _postRepository.GetPostById(token, postCreateDTO.ParentPostId ?? 0);

            var postid = await _postRepository.CreatePost(token, userid, postCreateDTO);

            var postURLS = new Dictionary<string, string>();

            try
            {
                foreach (var file in postCreateDTO.Images)
                {
                    var postURL = await _storageSupabase.UploadFile(token, userid, file, "posts");

                    postURLS.Add(file.FileName, postURL);
                }

                await _postRepository.InsertFile(token, postid, postURLS);
            }
            catch (Exception e)
            {
                await _postRepository.DeletePostById(token, postid);

                foreach (var file in postURLS)
                {
                    await _storageSupabase.DeleteFile(token, file.Value, "posts");
                }

                throw new InternalServerException("Não foi possível criar o post. Por favor, tente novamente mais tarde.");
            }

            return postid;
        }

        public async Task<List<PostResponse>> GetAllPosts(int page, int limit, string token, Guid userid)
        {
            return await _postRepository.GetAllPosts(page, limit, token, userid);
        }

        public async Task<int> GetTotalPosts(string token, Guid userid)
        {
            return await _postRepository.GetTotalPosts(token, userid);
        }

        public async Task<PostResponse> GetPostById(string token, int id)
        {
            return await _postRepository.GetPostById(token, id);
        }

        public async Task<CommentResponse> GetComments(string token, int id, int page, int limit)
        {
            return await _postRepository.GetComments(token, id, page, limit);
        }

        public async Task<int> GetTotalComments(string token, int id)
        {
            return await _postRepository.GetTotalComments(token, id);
        }

        public async Task<List<PostResponse>> GetUserPosts(string token, Guid userid, int page, int limit)
        {
            return await _postRepository.GetUserPosts(token, userid, page, limit);
        }

        public async Task<int> GetTotalUserPosts(string token, Guid userid)
        {
            return await _postRepository.GetTotalUserPosts(token, userid);
        }
    
        public async Task<List<PostResponse>> GetUserLikes(string token, Guid userid, int page, int limit)
        {
            return await _postRepository.GetUserLikes(token, userid, page, limit);
        }

        public async Task<int> GetTotalUserLikes(string token, Guid userid)
        {
            return await _postRepository.GetTotalUserLikes(token, userid);
        }
    }
}
