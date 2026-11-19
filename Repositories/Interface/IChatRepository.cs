using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Repositories.Interface
{
    public interface IChatRepository
    {
        public Task<ChatResponse> CreateChat(string token, Guid userid, CreateChatDTO chat);
    }
}
