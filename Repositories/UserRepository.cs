using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Supabase;

namespace Sabires_Backend_Application.Repositories
{
    public class UserRepository(SupabaseClienteService Client) : IUserRepository
    {
        public readonly Client client = Client.Client;

        public Task<string> CreateUser()
        {

        }
    }
}
