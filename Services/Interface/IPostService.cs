using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Services.Interface
{
    public interface IPostService
    {
        public Task<int> CreatePost(string token, Guid userid, PostCreateDTO postCreateDTO);
        public Task<PostResponse> GetPostById(string token, int id);
        public Task<int> GetTotalPosts(string token, Guid userid);
        public Task<List<PostResponse>> GetAllPosts(int page, int limit, string token, Guid userid);
        public Task<CommentResponse> GetComments(string token, int id, int page, int limit);
        public Task<int> GetTotalComments(string token, int id);
        public Task<List<PostResponse>> GetUserPosts(string token, Guid userid, int page, int limit);
        public Task<int> GetTotalUserPosts (string token, Guid userid);
        public Task<List<PostResponse>> GetUserLikes(string token, Guid userid, int page, int limit);
        public Task<int> GetTotalUserLikes(string token, Guid userid);
        public Task<List<PostResponse>> GetTrashPosts(string token, Guid userid, int page, int limit);
        public Task<int> GetTotalTrashPosts(string token, Guid userid);
        public Task LikePost(string token, Guid userid, int postid);
        public Task DislikePost(string token, Guid userid, int postid);
    }
}
