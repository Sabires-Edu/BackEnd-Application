using Microsoft.AspNetCore.Identity;
using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Utils;
using Sabires_Backend_Application.Utils.Exceptions;
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                BirthDate = userDTO.BirthDate
            };

            var UserId = await _userRepository.CreateUser(user);            

            return TokenGenerator.GenerateJwt(UserId, user.Email, "estudante");
        }

        public async Task<string> Login(LoginUserDTO userDTO)
        {
            var user = await _userRepository.GetPrivateUserByEmail(userDTO.Email);

            if (user.IsNull)
                throw new UnauthorizedException("Email ou senha incorretos");

            var result = Hasher.VerifyHashedPassword(null, user.User.Password, userDTO.Password);

            if(result == PasswordVerificationResult.Failed)
                throw new UnauthorizedException("Email ou senha incorretos");
            
            return TokenGenerator.GenerateJwt(user.User.UserId, user.User.Email, "estudante"); ;
        }
    
        public async Task<MeResponse> Me(string token)
        {
            return await _userRepository.Me(token);
        }
    }
}
