using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Services.Interface
{
    public interface IChatService
    {
        Task<List<ChatResponse>> GetMeChat(string token, Guid userid);
        Task<ChatResponse> GetChatById(string token, Guid userid, int chatid, int page, int limit);
        Task<ChatResponse> CreateChat(string token, Guid userid, CreateChatDTO chat);
        Task<List<MessageResponse>> GetChatMessages(string token, Guid userid, int chatid, int page, int limit);
        Task<int> GetTotalChatMessages(string token, int chatid);
        Task<int> GetTotalChatMembers(string token, int chatid);
        Task AddMemberToChat(string token, Guid userid, int chatid, Guid memberid);
        Task RemoveMemberFromChat(string token, Guid userid, int chatid, Guid memberid);
        Task UpdateChat(string token, Guid userid, int chatid, UpdateChatDTO chat);
        Task DeleteChat(string token, Guid userid, int chatid);
        Task<int> SendMessage(string token, Guid userid, int chatid, SendMessageDTO message);
    }
}
