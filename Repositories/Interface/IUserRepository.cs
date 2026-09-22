namespace Sabires_Backend_Application.Repositories.Interface
{
    public interface IUserRepository
    {
        public Task<string> CreateUser();
    }
}
