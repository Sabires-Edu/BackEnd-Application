using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Services.Interface
{
    public interface IUserService
    {
        public Task<string> CreateUser(CreateUserDTO user);
        public Task<string> Login(LoginUserDTO user);
        public Task<MeResponse> Me(string token);
    }
}
