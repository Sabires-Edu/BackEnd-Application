using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Services.Interface
{
    public interface IChatService
    {
        Task<ChatResponse> CreateChat(string token, Guid userid, CreateChatDTO chat);
    }
}
