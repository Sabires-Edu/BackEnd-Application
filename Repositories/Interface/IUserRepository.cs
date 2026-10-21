using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Repositories.Interface
{
    public interface IUserRepository
    {
        public Task<Guid> CreateUser(UserStudent user);
        public Task<UserByEmailResponse> GetUserByEmail(string email);
        public Task<PrivateUserByEmailResponse> GetPrivateUserByEmail(string email);
        public Task<MeResponse> Me(string token);
        public Task<UserResponse> GetUserById(string userid, string token);
        public Task<List<UserResponse>> GetAllUsers(int page, int limit, string token, Guid userid);
        public Task<int> GetTotalUsers(string token);
    }
}
