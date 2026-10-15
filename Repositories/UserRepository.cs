using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Utils.Exceptions;
using Supabase;
using System.Text.Json;

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

                if (response.Content.IsNullOrEmpty())
                    throw new Exception();                

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

            if(response.Models.Count == 0)
                throw new NotFoundException("Usuário não encontrado.");

            var user = response.Models.First();

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
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
