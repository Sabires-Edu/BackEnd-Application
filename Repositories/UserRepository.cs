using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Utils.Deserialize;
using Sabires_Backend_Application.Utils.Exceptions;
using Supabase;
using System.Text.Json;
using static Supabase.Postgrest.Constants;

namespace Sabires_Backend_Application.Repositories
{
    public class UserRepository(SupabaseClientService Client) : IUserRepository
    {
        public readonly Client client = Client.Client;
        public readonly Client adminClient = Client.AdminClient;

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

        public async Task<PrivateUserByEmailResponse?> GetPrivateUserByEmail(string email)
        {
            if (adminClient == null)
                throw new Exception("Cliente é nulo.");

            var response = await adminClient.From<User>().Where(user => user.Email == email).Get();

            if (response.Models.Count == 0)
                return new PrivateUserByEmailResponse { IsNull = true };

            return new PrivateUserByEmailResponse { User = response.Models.First(), IsNull = false };
        }

        public async Task<MeResponse> Me(string token)
        {
            Client client = Client.InitCustomClient(token);

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
                TotalFollowers = totalFollowers,
                TotalFollowing = totalFollowing
            };
        }

        public async Task<UserResponse> GetUserById(string userid, string token)
        {
            Client client = Client.InitCustomClient(token);

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
                TotalFollowers = totalFollowers,
                TotalFollowing = totalFollowing
            };
        }

        public async Task<List<UserResponse>> GetAllUsers(int page, int limit, string token, Guid userid)
        {
            Client client = Client.InitCustomClient(token);

            int inicio = (page - 1) * limit;
            int fim = inicio + limit - 1;

            var response = await client.From<PublicUser>().Range(inicio, fim).Where(user => user.UserId != userid).Get();

            return response.Models.Select(user => DeserializeUserResponse.DeserializeUsers(user)).ToList();
        }

        public async Task<int> GetTotalUsers(string token)
        {
            Client client = Client.InitCustomClient(token);

            var total = await client.From<PublicUser>().Count(CountType.Exact);

            return total;
        }

        public async Task<List<UserResponse>> GetFollowers(int page, int limit, string token, Guid userid)
        {
            Client client = Client.InitCustomClient(token);

            int inicio = (page - 1) * limit;
            int fim = inicio + limit - 1;

            //var test = await client.From<Follower>().Where(follower => follower.FollowerUserId == user.UserId).Get();

            var followerResponse = await adminClient.From<Follower>().Select("""*, users!fk_users_followeruserid(*)""").Range(inicio, fim).Where(follower => follower.FollowerUserId == userid).Get();

            Console.WriteLine("Teste");

            return followerResponse.Models.Select(follower => DeserializeUserResponse.DeserializeUsers(follower.)).ToList();
        }

        public async Task<List<UserResponse>> GetFollowings(int page, int limit, string token, Guid userid)
        {
            Client client = Client.InitCustomClient(token);

            int inicio = (page - 1) * limit;
            int fim = inicio + limit - 1;

            var followingResponse = await client.From<Follower>().Select("""*, followinguser:publicuser!FK_Users_FollowingUserId(*)""").Range(inicio, fim).Where(follower => follower.FollowingUserId == userid).Get();

            return followingResponse.Models.Select(follower => DeserializeUserResponse.DeserializeUsers(follower.FollowingUser)).ToList();
        }

        public async Task<int> GetTotalFollowerUsers(string token, Guid userid, string type)
        {
            Client client = Client.InitCustomClient(token);

            var total = await client.From<Follower>().Where(type == "follower" ? follower => follower.FollowerUserId == userid : following => following.FollowingUserId == userid).Count(CountType.Exact);

            return total;
        }
    }
}
