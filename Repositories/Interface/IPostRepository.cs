using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Repositories.Interface
{
    public interface IPostRepository
    {
        public Task<int> CreatePost(string token, Guid userid, PostCreateDTO postCreateDTO);
        public Task<PostResponse> GetPostById(string token, int id);
        public Task DeletePostById(string token, int id);
        public Task InsertFile(string token, int postid, Dictionary<string, string> files);
        public Task<List<PostResponse>> GetAllPosts(int page, int limit, string token, Guid userid);
        public Task<int> GetTotalPosts(string token, Guid userid);
    }
}
