using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Utils.Exceptions;
using Sabires_Backend_Application.Utils.Validation;

namespace Sabires_Backend_Application.Services
{
    public class ChatService(IChatRepository postRespository, IStorageSupabase storageSupabase, IUserRepository userRepository) : IChatService
    {
        private readonly ChatValidation _validation = new ChatValidation();

        public async Task<List<ChatResponse>> GetMeChat(string token, Guid userid)
        {
            return await postRespository.GetMeChat(token, userid);
        }

        public async Task<ChatResponse> GetChatById(string token, Guid userid, int chatid, int page, int limit)
        {
            return await postRespository.GetChatById(token, userid, chatid, page, limit);
        }

        public async Task<ChatResponse> CreateChat(string token, Guid userid, CreateChatDTO chat)
        {
            await _validation.CheckDTO(token, chat, userRepository);            
            return await postRespository.CreateChat(token, userid, chat);
        }
    
        public async Task<List<MessageResponse>> GetChatMessages(string token, Guid userid, int chatid, int page, int limit)
        {
            return await postRespository.GetChatMessages(token, chatid, userid, page, limit);
        }
    
        public async Task<int> GetTotalChatMessages(string token, int chatid)
        {
            return await postRespository.GetTotalChatMessages(token, chatid);
        }

        public async Task<int> GetTotalChatMembers(string token, int chatid)
        {
            return await postRespository.GetTotalChatMembers(token, chatid);
        }
    }
}
