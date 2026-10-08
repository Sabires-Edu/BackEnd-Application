using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Supabase;

namespace Sabires_Backend_Application.Repositories
{
    public class UserRepository(SupabaseClienteService Client) : IUserRepository
    {
        public readonly Client client = Client.Client;
        public readonly Client adminClient = Client.AdminClient;

        public async Task<Guid> CreateUser(UserStudent user)
        {
            Guid userid;

            try
            {
                var response = await adminClient.Rpc("create_user_student", user);
   
                userid = Guid.Parse(response.Content ?? throw new Exception("Ocorreu um erro interno."));
            }
            catch (Exception e)
            {
                throw new Exception("Ocorreu um erro.");
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
    }
}
