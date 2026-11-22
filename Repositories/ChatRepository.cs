using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Utils.Exceptions;
using Supabase.Gotrue;
using Supabase.Postgrest;
using System.Text.Json;
using static Supabase.Postgrest.Constants;

namespace Sabires_Backend_Application.Repositories
{
    public class ChatRepository(SupabaseClientService Client) : IChatRepository
    {
        public readonly SupabaseClientService _client = Client;
        public readonly Supabase.Client adminclient = Client.AdminClient;

        public async Task<List<ChatResponse>> GetMeChat(string token, Guid userid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await adminclient.Rpc("get_me_chats", new { userid_param = userid });

                if (string.IsNullOrWhiteSpace(response.Content))
                    throw new InternalServerException("Não foi possível buscar os chats do usuário. Por favor, tente novamente mais tarde.");

                var chatMembers = JsonSerializer.Deserialize<List<ChatResponse>>(response.Content);

                return chatMembers;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar os chats do usuário. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ChatResponse> GetChatById(string token, Guid userid, int chatid, int page, int limit)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var chatResponse = await client.From<Chat>().Where(ct => ct.ChatId == chatid).Get();

                if (string.IsNullOrWhiteSpace(chatResponse.Content))
                    throw new InternalServerException("Não foi possível buscar os detalhes do chat. Por favor, tente novamente mais tarde.");

                var chatDetails = JsonSerializer.Deserialize<List<ChatResponse>>(chatResponse.Content);

                var chat = chatDetails.FirstOrDefault();

                var memberResponse = await adminclient.From<ChatMember>().Where(cm => cm.ChatId == chatid).Range((page - 1) * limit, page * limit - 1).Order("publicuser(firstname)", Ordering.Ascending).Order("publicuser(lastname)", Ordering.Ascending).Get();

                if (string.IsNullOrWhiteSpace(memberResponse.Content))
                    throw new InternalServerException("Não foi possível buscar os membros do chat. Por favor, tente novamente mais tarde.");

                var chatMembers = JsonSerializer.Deserialize<List<ChatMembersResponse>>(memberResponse.Content);

                var members = new List<UserResponse>();

                foreach (var member in chatMembers)
                {
                    members.Add(member.Members);
                }

                chat.Members = members;

                return chat;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new InternalServerException("Não foi possível buscar os detalhes do chat. Por favor, tente novamente mais tarde.");
            }
        }

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

        public async Task<List<MessageResponse>> GetChatMessages(string token, int chatid, Guid userid, int page, int limit)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var chatResponse = await adminclient.From<ChatMember>().Where(cm => cm.ChatId == chatid && cm.UserId == userid).Count(CountType.Exact);

                if (chatResponse == 0)
                    throw new BadRequestException("O usuário não é membro do chat. Por favor, entre em contato com o administrador do chat.");

                var response = await client.From<Message>().Where(msg => msg.ChatId == chatid).Range((page - 1) * limit, page * limit - 1).Order("sentat", Ordering.Descending).Get();

                if (string.IsNullOrWhiteSpace(response.Content))
                    throw new InternalServerException("Não foi possível buscar as mensagens do chat. Por favor, tente novamente mais tarde.");

                var messages = JsonSerializer.Deserialize<List<MessageResponse>>(response.Content);

                return messages;
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new InternalServerException("Não foi possível buscar as mensagens do chat. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetTotalChatMessages(string token, int chatid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await client.From<Message>().Where(m => m.ChatId == chatid).Count(CountType.Exact);

                return response;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar a quantidade de mensagens do chat. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetTotalChatMembers(string token, int chatid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var response = await adminclient.From<ChatMember>().Where(cm => cm.ChatId == chatid).Count(CountType.Exact);

                return response;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Não foi possível buscar a quantidade de membros do chat. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task AddMemberToChat(string token, Guid userid, int chatid, Guid memberid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var memberResponse = await client.From<PublicUser>().Where(pu => pu.UserId == memberid).Count(CountType.Exact);

                if (memberResponse == 0)
                    throw new BadRequestException("Não foi possível encontrar esse usuário. Por favor adicione um membro existente.");

                var ownerResponse = await client.From<Chat>().Where(ct => ct.OwnerId == userid && ct.ChatId == chatid).Count(CountType.Exact);

                if (ownerResponse == 0)
                    throw new BadRequestException("Você não é o dono do chat. Por favor peça para o dono realizar a operação");

                var chatMemberResponse = await adminclient.From<ChatMember>().Where(cm => cm.ChatId == chatid && cm.UserId == memberid).Count(CountType.Exact);

                if (chatMemberResponse > 0)
                    throw new BadRequestException("O membro já foi adicionado ao chat. Por favor adicione outro usuário como membro.");

                var newMember = new ChatMember
                {
                    ChatId = chatid,
                    UserId = memberid
                };

                var response = await adminclient.From<ChatMember>().Insert(newMember);

                if (string.IsNullOrWhiteSpace(response.Content))
                    throw new InternalServerException("Houve um erro ao inserir o novo membro ao chat. Por favor tente novamente mais tarde.");
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Houve um erro ao inserir o novo membro ao chat. Por favor tente novamente mais tarde.");
            }
        }

        public async Task RemoveMemberFromChat(string token, Guid userid, int chatid, Guid memberid)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                if (userid == memberid)
                    throw new BadRequestException("Não é possível remover a si próprio do grupo. Por favor remova outros membros.");

                var memberResponse = await client.From<PublicUser>().Where(pu => pu.UserId == memberid).Count(CountType.Exact);

                if (memberResponse == 0)
                    throw new BadRequestException("Não foi possível encontrar esse usuário. Por favor adicione um membro existente.");

                var ownerResponse = await client.From<Chat>().Where(ct => ct.OwnerId == userid && ct.ChatId == chatid).Count(CountType.Exact);

                if (ownerResponse == 0)
                    throw new BadRequestException("Você não é o dono do chat. Por favor peça para o dono realizar a operação");

                var chatMemberResponse = await adminclient.From<ChatMember>().Where(cm => cm.ChatId == chatid && cm.UserId == memberid).Count(CountType.Exact);

                if (chatMemberResponse == 0)
                    throw new BadRequestException("O membro não está presente no chat. Por favor remova outro membro.");

                var response = await adminclient.From<ChatMember>().Where(cm => cm.UserId == memberid).Delete();

                if (string.IsNullOrWhiteSpace(response.Content))
                    throw new InternalServerException("Houve um erro ao remover o membro ao chat. Por favor tente novamente mais tarde.");
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Houve um erro ao remover o membro ao chat. Por favor tente novamente mais tarde.");
            }
        }
    
        public async Task UpdateChat(string token, Guid userid, int chatid, UpdateChatDTO chat)
        {
            try
            {
                Supabase.Client client = _client.InitCustomClient(token);

                var ownerResponse = await client.From<Chat>().Where(ct => ct.OwnerId == userid && ct.ChatId == chatid).Count(CountType.Exact);

                if (ownerResponse == 0)
                    throw new BadRequestException("Você não é o dono do chat. Por favor peça para o dono realizar a operação");

                var updateChat = new Chat
                {
                    ChatName = chat.ChatName,
                    ChatDescription = chat.ChatDescription,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                var query = adminclient.From<Chat>().Where(ct => ct.ChatId == chatid);

                if (!string.IsNullOrEmpty(chat.ChatName))
                    query = query.Set(u => u.ChatName, chat.ChatName);

                if (!string.IsNullOrEmpty(chat.ChatDescription))
                    query = query.Set(u => u.ChatDescription, chat.ChatDescription);

                query = query.Set(u => u.UpdatedAt, DateTimeOffset.UtcNow);

                var response = await query.Update();

                if (string.IsNullOrWhiteSpace(response.Content))
                    throw new InternalServerException("Houve um erro ao atualizar o chat. Por favor tente novamente mais tarde.");
            }
            catch (BadRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Houve um erro ao atualizar o chat. Por favor tente novamente mais tarde.");
            }
        }
    }
}
