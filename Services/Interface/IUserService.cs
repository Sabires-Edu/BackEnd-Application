using Sabires_Backend_Application.Models.DTO;

namespace Sabires_Backend_Application.Services.Interface
{
    public interface IUserService
    {
        public Task<string> CreateUser(CreateUserDTO user);

        public Task<string> Login(LoginUserDTO user);
    }
}
