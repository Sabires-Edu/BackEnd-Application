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
    }
}
