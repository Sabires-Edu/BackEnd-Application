using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Utils.Exceptions;
using Supabase;
using Supabase.Postgrest;
using System.Text.Json;
using static Supabase.Postgrest.Constants;

namespace Sabires_Backend_Application.Repositories
{
    public class UserRepository(SupabaseClientService Client) : IUserRepository
    {
        public readonly Supabase.Client client = Client.Client;
        public readonly Supabase.Client adminClient = Client.AdminClient;

        public async Task<Guid> CreateUser(UserStudent user)
        {
            Guid userid;

            try
            {
                var response = await adminClient.Rpc("create_user_student", user);

                if (string.IsNullOrEmpty(response.Content))
                    throw new InternalServerException("Não foi possível criar o novo usuário. Por favor, tente novamente mais tarde.");

                userid = JsonSerializer.Deserialize<Guid>(response.Content);
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível criar o novo usuário. Por favor, tente novamente mais tarde.");
            }

            return userid;
        }

        public async Task<UserByEmailResponse?> GetUserByEmail(string email)
        {
            if (client == null)
                throw new Exception("Cliente é nulo.");

            var response = await client.From<PublicUser>().Where(user => user.Email == email).Get();

            if (response.Models.Count == 0)
                return new UserByEmailResponse { IsNull = true };

            return new UserByEmailResponse { User = response.Models.First(), IsNull = false };
        }

        public async Task<PrivateUserByEmailResponse?> GetPrivateUserByEmailOrId(string email = "", Guid id = default(Guid))
        {
            if (adminClient == null)
                throw new Exception("Cliente é nulo.");

            var response = await adminClient.From<User>().Where(user => (email != "" ? user.Email == email : user.UserId == id)).Get();

            if (response.Models.Count == 0)
                return new PrivateUserByEmailResponse { IsNull = true };

            return new PrivateUserByEmailResponse { User = response.Models.First(), IsNull = false };
        }

        public async Task<MeResponse> Me(string token)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            var response = await client.From<User>().Get();

            if (response.Models.Count == 0)
                throw new NotFoundException("Usuário não encontrado.");

            var user = response.Models.First();

            var totalFollowing = await client.From<Follower>().Where(follower => follower.FollowerUserId == user.UserId && follower.StatusId == 2).Count(CountType.Exact);
            var totalFollowers = await client.From<Follower>().Where(follower => follower.FollowingUserId == user.UserId && follower.StatusId == 2).Count(CountType.Exact);

            return new MeResponse
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture,
                Biography = user.Biography,
                StatusId = user.StatusId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                TotalFollowers = totalFollowers == 0 ? 0 : totalFollowers,
                TotalFollowing = totalFollowing == 0 ? 0 : totalFollowing
            };
        }

        public async Task<UserResponse> GetUserById(string userid, string token)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            PublicUser user = new PublicUser();

            try
            {
                var response = await client.From<PublicUser>().Where(user => user.UserId == Guid.Parse(userid)).Get();

                if (response.Models.Count == 0)
                    throw new NotFoundException("Usuário não encontrado.");

                user = response.Models.FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new NotFoundException("Usuário não encontrado. Por favor, selecione um usuário válido.");
            }

            var totalFollowers = await client.From<Follower>().Where(follower => follower.FollowerUserId == user.UserId).Count(CountType.Exact);
            var totalFollowing = await client.From<Follower>().Where(follower => follower.FollowingUserId == user.UserId).Count(CountType.Exact);

            return new UserResponse
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture,
                Biography = user.Biography,
                StatusId = user.StatusId,
                TotalFollowers = totalFollowers == 0 ? 0 : totalFollowers,
                TotalFollowing = totalFollowing == 0 ? 0 : totalFollowing
            };
        }

        public async Task<List<UserResponse>> GetAllUsers(int page, int limit, string token, Guid userid)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            int inicio = (page - 1) * limit;

            var config = new GetUsers
            {
                Inicio = inicio,
                Limite = limit,
                Id = userid
            };

            var response = await client.Rpc("get_all_users", config);

            var users = JsonSerializer.Deserialize<List<UserFunction>>(response.Content);

            return users.Select(user => new UserResponse
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture,
                Biography = user.Biography,
                StatusId = user.StatusId,
                TotalFollowers = user.Followers == 0 ? 0 : user.Followers,
                TotalFollowing = user.Followings == 0 ? 0 : user.Followings
            }).ToList();
        }

        public async Task<int> GetTotalUsers(string token)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            var total = await client.From<PublicUser>().Count(CountType.Exact);

            return total;
        }

        public async Task<List<UserResponse>> GetFollowers(int page, int limit, string token, Guid userid)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            int inicio = (page - 1) * limit;

            var config = new GetUsers
            {
                Inicio = inicio,
                Limite = limit,
                Id = userid
            };

            var response = await client.Rpc("get_all_followers", config);

            var users = JsonSerializer.Deserialize<List<UserFunction>>(response.Content);

            return users.Select(user => new UserResponse
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture,
                Biography = user.Biography,
                StatusId = user.StatusId,
                TotalFollowers = user.Followers == 0 ? 0 : user.Followers,
                TotalFollowing = user.Followings == 0 ? 0 : user.Followings
            }).ToList();
        }

        public async Task<List<UserResponse>> GetFollowings(int page, int limit, string token, Guid userid)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            int inicio = (page - 1) * limit;

            var config = new GetUsers
            {
                Inicio = inicio,
                Limite = limit,
                Id = userid
            };

            var response = await client.Rpc("get_all_followings", config);

            var users = JsonSerializer.Deserialize<List<UserFunction>>(response.Content);

            return users.Select(user => new UserResponse
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture,
                Biography = user.Biography,
                StatusId = user.StatusId,
                TotalFollowers = user.Followers == 0 ? 0 : user.Followers,
                TotalFollowing = user.Followings == 0 ? 0 : user.Followings
            }).ToList();
        }

        public async Task<int> GetTotalFollowerUsers(string token, Guid userid, string type)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            var total = await client.From<Follower>().Where(type == "follower" ?
                (follower => follower.FollowingUserId == userid && follower.StatusId == 2) :
                (following => following.FollowerUserId == userid && (following.StatusId == 1 || following.StatusId == 2)))
                .Count(CountType.Exact);

            return total;
        }

        public async Task<int> FollowUser(string token, Guid followerUserId, Guid followingUserId)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            var followerBody = new Follower
            {
                FollowerUserId = followerUserId,
                FollowingUserId = followingUserId,
                StatusId = 1
            };

            var response = await client.From<Follower>().Insert(followerBody, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

            if (string.IsNullOrEmpty(response.Content))
                throw new InternalServerException("Não foi possível seguir o usuário. Por favor, tente novamente mais tarde.");

            var result = JsonSerializer.Deserialize<List<FollowerResponse>>(response.Content);

            return result[0].FollowersId;
        }

        public async Task<List<UserResponse>> GetPendingFollows(int page, int limit, string token, Guid userid)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            int inicio = (page - 1) * limit;

            var config = new GetUsers
            {
                Inicio = inicio,
                Limite = limit,
                Id = userid
            };

            var response = await client.Rpc("get_pending_follows", config);

            var users = JsonSerializer.Deserialize<List<UserFunction>>(response.Content);

            return users.Select(user => new UserResponse
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture,
                Biography = user.Biography,
                StatusId = user.StatusId,
                TotalFollowers = user.Followers == 0 ? 0 : user.Followers,
                TotalFollowing = user.Followings == 0 ? 0 : user.Followings
            }).ToList();
        }

        public async Task<int> GetTotalPendingFollows(string token, Guid userid)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            var total = await client.From<Follower>().Where(follower => follower.FollowingUserId == userid && follower.StatusId == 1)
                .Count(CountType.Exact);

            return total;
        }

        public async Task UpdateFollowStatus(string token, Guid followerUserId, Guid followingUserId, int status)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            var response = await client.From<Follower>().Where(follower => follower.FollowerUserId == followerUserId && follower.FollowingUserId == followingUserId && follower.StatusId == 1).Set(follower => follower.StatusId, status).Update();
        }

        public async Task<StudentById> GetStudentById(Guid userid, string token)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            var config = new GetStudent
            {
                UserId = userid
            };

            var response = await client.Rpc("get_user_student", config);

            if (string.IsNullOrEmpty(response.Content))
                throw new NotFoundException("Estudante não encontrado.");

            var student = JsonSerializer.Deserialize<List<StudentById>>(response.Content);

            if (student == null || student.Count == 0)
                throw new InternalServerException("Houve um erro ao carregar os dados do estudante. Por favor tente novamente mais tarde.");

            return student[0];
        }

        public async Task<StudentById> UpdateUser(string token, UpdateUserDTO user, Guid userId)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            var query = client.From<User>().Where(u => u.UserId == userId);
            DateTimeOffset BirthDate = new DateTimeOffset();

            if (!string.IsNullOrEmpty(user.FirstName))
                query = query.Set(u => u.FirstName, user.FirstName);

            if (!string.IsNullOrEmpty(user.LastName))
                query = query.Set(u => u.LastName, user.LastName);

            if (user.StatusId != null)
                query = query.Set(u => u.StatusId, user.StatusId);

            if (!string.IsNullOrEmpty(user.Biography))
                query = query.Set(u => u.Biography, user.Biography); 

            if (user.BirthDate != null)
            {
                var studentQuery = client.From<Student>().Where(s => s.UserId == userId).Set(s => s.BirthDate, user.BirthDate);   
                
                var studentResponse = await studentQuery.Update(new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

                if (string.IsNullOrEmpty(studentResponse.Content))
                    throw new InternalServerException("Não foi possível atualizar o usuário. Por favor, tente novamente mais tarde.");

                var StudentUpdated = studentResponse.Models.First();

                BirthDate = StudentUpdated.BirthDate;
            }

            query = query.Set(u => u.UpdatedAt, DateTimeOffset.UtcNow);

            var response = await query.Update(new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

            if (string.IsNullOrEmpty(response.Content))
                throw new InternalServerException("Não foi possível atualizar o usuário. Por favor, tente novamente mais tarde.");

            var updatedUser = response.Models.First();

            return new StudentById
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Biography = user.Biography,
                StatusId = user.StatusId ?? 0,
                BirthDate = BirthDate
            };
        }
    
        public async Task UpdatePassword(string token, string hashedPassword, Guid userid)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            var response = await client.From<User>().Where(user => user.UserId == userid).Set(user => user.Password, hashedPassword).Update();

            if (string.IsNullOrEmpty(response.Content))
                throw new InternalServerException("Não foi possível atualizar a senha do usuário. Por favor, tente novamente mais tarde.");
        }
    
        public async Task UpdateProfilePicture(string token, Guid userid, string profilePictureUrl)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            var response = await client.From<User>().Where(user => user.UserId == userid).Set(user => user.ProfilePicture, profilePictureUrl).Update();

            if (string.IsNullOrEmpty(response.Content))
                throw new InternalServerException("Não foi possível atualizar a foto de perfil do usuário. Por favor, tente novamente mais tarde.");
        }
    }
}
