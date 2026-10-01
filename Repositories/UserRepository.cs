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

        public async Task<Guid> CreateUser(User user)
        {
            Console.WriteLine(user.FirstName);
            Console.WriteLine(user.LastName);
            Console.WriteLine(user.Email);

            var response = await adminClient.From<User>().Insert(user, new Supabase.Postgrest.QueryOptions { Returning = Supabase.Postgrest.QueryOptions.ReturnType.Representation });

            var createdUser = response.Models.First() ?? throw new Exception("Ocorreu algum erro na criação do usuário. Por favor tente novamente mais tarde.");

            return createdUser.UserId;
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
