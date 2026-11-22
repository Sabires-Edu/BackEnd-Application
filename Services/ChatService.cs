using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services.Interface;
using Sabires_Backend_Application.Utils.Exceptions;
using Sabires_Backend_Application.Utils.Validation;

namespace Sabires_Backend_Application.Services
{
    public class ChatService(IChatRepository chatRepository, IStorageSupabase storageSupabase, IUserRepository userRepository) : IChatService
    {
        private readonly ChatValidation _validation = new ChatValidation();

        public async Task<List<ChatResponse>> GetMeChat(string token, Guid userid)
        {
            return await chatRepository.GetMeChat(token, userid);
        }

        public async Task<ChatResponse> GetChatById(string token, Guid userid, int chatid, int page, int limit)
        {
            return await chatRepository.GetChatById(token, userid, chatid, page, limit);
        }

        public async Task<ChatResponse> CreateChat(string token, Guid userid, CreateChatDTO chat)
        {
            await _validation.CheckDTO(token, chat, userRepository);            
            return await chatRepository.CreateChat(token, userid, chat);
        }
    
        public async Task<List<MessageResponse>> GetChatMessages(string token, Guid userid, int chatid, int page, int limit)
        {
            return await chatRepository.GetChatMessages(token, chatid, userid, page, limit);
        }
    
        public async Task<int> GetTotalChatMessages(string token, int chatid)
        {
            return await chatRepository.GetTotalChatMessages(token, chatid);
        }

        public async Task<int> GetTotalChatMembers(string token, int chatid)
        {
            return await chatRepository.GetTotalChatMembers(token, chatid);
        }
    
        public async Task AddMemberToChat(string token, Guid userid, int chatid, Guid memberid)
        {
            await chatRepository.AddMemberToChat(token, userid, chatid, memberid);
        }

        public async Task RemoveMemberFromChat(string token, Guid userid, int chatid, Guid memberid)
        {
            await chatRepository.RemoveMemberFromChat(token, userid, chatid, memberid);
        }

        public async Task UpdateChat(string token, Guid userid, int chatid, UpdateChatDTO chat)
        {
            await _validation.CheckUpdateDTO(token, chat, chatid, userid, chatRepository);
            await chatRepository.UpdateChat(token, userid, chatid, chat);
        }
 
        public async Task DeleteChat(string token, Guid userid, int chatid)
        {
            await chatRepository.DeleteChat(token, userid, chatid);
        }
   
        public async Task<int> SendMessage(string token, Guid userid, int chatid, SendMessageDTO message)
        {
            await _validation.CheckSendMessageDTO(token, message, chatid, userid, chatRepository);
            return await chatRepository.SendMessage(token, userid, chatid, message);
        }
    }
}
