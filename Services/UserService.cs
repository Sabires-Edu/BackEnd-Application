using Microsoft.AspNetCore.Identity;
using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Utils.Exceptions;
using Sabires_Backend_Application.Utils.Token;
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

        public async Task<UserResponse> GetUserById(string userid, string token) {
            return await _userRepository.GetUserById(userid, token);
        }
    
        public async Task<List<UserResponse>> GetAllUsers(int page, int limit, string token, Guid userid)
        {
            return await _userRepository.GetAllUsers(page, limit, token, userid);
        }

        public async Task<int> GetTotalUsers(string token)
        {
            return await _userRepository.GetTotalUsers(token);
        }

        public async Task<List<UserResponse>> GetFollowers(int page, int limit, string token, Guid userid)
        {
            return await _userRepository.GetFollowers(page, limit, token, userid);
        }

        public async Task<List<UserResponse>> GetFollowings(int page, int limit, string token, Guid userid)
        {
            return await _userRepository.GetFollowings(page, limit, token, userid);
        }

        public async Task<int> GetTotalFollowerUsers(string token, Guid userid, string type)
        {
            return await _userRepository.GetTotalFollowerUsers(token, userid, type);
        }
    
        public async Task<int> FollowUser(string token, Guid followerUserId, Guid followingUserId)
        {
            var followerUser = await _userRepository.GetUserById(followingUserId.ToString(), token);

            return await _userRepository.FollowUser(token, followerUserId, followingUserId);
        }

        public async Task<List<UserResponse>> GetPendingFollows(int page, int limit, string token, Guid userid)
        {
            return await _userRepository.GetPendingFollows(page, limit, token, userid);
        }

        public async Task<int> GetTotalPendingFollows(string token, Guid userid)
        {
            return await _userRepository.GetTotalPendingFollows(token, userid);
        }

        public async Task UpdateFollowStatus(string token, Guid followerUserId, Guid followingUserId, int status)
        {
            if(status != 2 && status != 3)
                throw new BadRequestException("Status inválido. Por favor insira um status válido.");
                
            await _userRepository.UpdateFollowStatus(token, followerUserId, followingUserId, status);
        }

        public async Task<StudentById> UpdateUser(string token, UpdateUserDTO user, Guid userid)
        {
            var currentUser = await _userRepository.GetStudentById(userid, token);
            UserValidation.ValidateUpdateUser(user, currentUser, _userRepository);
            return await _userRepository.UpdateUser(token, user, userid);
        }
    }
}
