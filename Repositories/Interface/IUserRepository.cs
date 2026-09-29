using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Repositories.Interface
{
    public interface IUserRepository
    {
        public Task<Guid> CreateUser(User user);

        public Task<UserByEmailResponse> GetUserByEmail(string email);
    }
}
