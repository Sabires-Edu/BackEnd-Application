using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services.Interface;

namespace Sabires_Backend_Application.Services
{
    public class UserService (IUserRepository userRepository) : IUserService
    {
        public readonly IUserRepository _userRepository = userRepository;

        public async Task<string> CreateUser()
        {
            var result = await _userRepository.CreateUser();

            return result;
        }
    }
}
