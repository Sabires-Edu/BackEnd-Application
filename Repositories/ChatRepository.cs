using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Utils.Exceptions;
using System.Text.Json;

namespace Sabires_Backend_Application.Repositories
{
    public class ChatRepository(SupabaseClientService Client) : IChatRepository
    {
        public readonly SupabaseClientService _client = Client;
        public readonly Supabase.Client adminclient = Client.AdminClient;

        public async Task<ChatResponse> CreateChat(string token, Guid userid, CreateChatDTO chat)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                List<Guid> listMembers = new List<Guid> { };

                foreach (var memberId in chat.MembersId)
                {
                    if (!Guid.TryParse(memberId, out Guid parsedMemberId))
                        throw new BadRequestException("Não foi possível converter os IDs do usuário. Por favor tente com IDs válidos.");

                    listMembers.Add(parsedMemberId);
                }

                listMembers.Add(userid);

                var chatConfig = new ChatConfig
                {
                    ChatTypeId = chat.ChatTypeId,
                    OwnerId = userid,
                    ChatName = chat.ChatName,
                    ChatDescription = chat.ChatDescription,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                    MembersId = listMembers
                };

                var response = await adminclient.Rpc("create_chat", chatConfig);

                var chatResponse = JsonSerializer.Deserialize<List<ChatResponse>>(response.Content);

                return chatResponse[0];
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new InternalServerException("Não foi possível criar o chat. Por favor, tente novamente mais tarde.");
            }
        }
    }
}
