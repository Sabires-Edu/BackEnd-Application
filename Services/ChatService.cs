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
        public async Task<ChatResponse> CreateChat(string token, Guid userid, CreateChatDTO chat)
        {
            await _validation.CheckDTO(token, chat, userRepository);            
            return await postRespository.CreateChat(token, userid, chat);
        }
    }
}
