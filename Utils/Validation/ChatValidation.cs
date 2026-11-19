using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Repositories;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Utils.Exceptions;

namespace Sabires_Backend_Application.Utils.Validation
{
    public class ChatValidation
    {
        public async Task CheckDTO(string token, CreateChatDTO chat, IUserRepository userRepository)
        {
            var chatTypes = new List<int> { 1, 2 };

            if (!chatTypes.Contains(chat.ChatTypeId))
                throw new BadRequestException("O tipo de chat é inválido. Por favor tente novamente.");

            if (string.IsNullOrEmpty(chat.ChatName))
                throw new BadRequestException("O nome do chat é obrigatório para a criação de um chat em grupo.");

            if (chat.ChatName.Length > 200)
                throw new BadRequestException("O nome do chat não pode ter mais de 200 caracteres.");

            if (!string.IsNullOrWhiteSpace(chat.ChatDescription) && chat.ChatDescription.Length > 400)
                throw new BadRequestException("A descrição do chat não pode ter mais de 400 caracteres.");

             if(chat.MembersId.Count > 0)
            {
                foreach(var memberid in chat.MembersId)
                {
                    await userRepository.GetUserById(memberid, token);
                }
            }
        }
    }
}
