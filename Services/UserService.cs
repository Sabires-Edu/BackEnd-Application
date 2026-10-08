using Microsoft.AspNetCore.Identity;
using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Utils;
using Sabires_Backend_Application.Utils.Validation;

namespace Sabires_Backend_Application.Services
{
    public class UserService(IUserRepository userRepository, TokenGenerator tokenGenerator) : IUserService
    {
        public readonly IUserRepository _userRepository = userRepository;
        private readonly PasswordHasher<string> Hasher = new PasswordHasher<string>();

        private readonly TokenGenerator TokenGenerator = tokenGenerator;

        public async Task<string> CreateUser(CreateUserDTO userDTO)
        {
            UserValidation.ValidateUserInput(userDTO, _userRepository);

            string HashedPassword = Hasher.HashPassword(null, userDTO.Password);

            UserStudent user = new UserStudent
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                Password = HashedPassword,
                RoleId = 1,
                StatusId = userDTO.StatusId,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                BirthDate = userDTO.BirthDate
            };

            var UserId = await _userRepository.CreateUser(user);

            var jwt = TokenGenerator.GenerateJwt(UserId, user.Email, "estudante");

            return jwt;
        }
    }
}
