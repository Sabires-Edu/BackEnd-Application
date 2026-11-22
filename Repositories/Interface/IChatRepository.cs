using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Repositories.Interface
{
    public interface IChatRepository
    {
        public Task<List<ChatResponse>> GetMeChat(string token, Guid userid);
        public Task<ChatResponse> GetChatById(string token, Guid userid, int chatid, int page, int limit);
        public Task<ChatResponse> CreateChat(string token, Guid userid, CreateChatDTO chat);
        public Task<List<MessageResponse>> GetChatMessages(string token, int chatid, Guid userid, int page, int limit);   
        public Task<int> GetTotalChatMessages(string token, int chatid);
        public Task<int> GetTotalChatMembers(string token, int chatid);
        public Task AddMemberToChat(string token, Guid userid, int chatid, Guid memberid);
        public Task RemoveMemberFromChat(string token, Guid userid, int chatid, Guid memberid);         
        public Task UpdateChat(string token, Guid userid, int chatid, UpdateChatDTO chat);
        public Task DeleteChat(string token, Guid userid, int chatid);
    }
}
