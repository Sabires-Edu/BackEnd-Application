using System.Reflection;
using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models.Response
{
    public class SuccessResponse
    {        
        // Successo da Requisição
        public required bool Success { get; set; }

        // Descrição breve do erro
        public required string Title { get; set; }

        // Código de status HTTP
        public required int Status { get; set; }

        // Conteúdo da resposta
        public required Response Response { get; set; }
    }

    public class Response
    {
        // Token de autenticação
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]        
        public string? JWT { get; set; }

        // Informações do usuário logado
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MeResponse? Me { get; set; }

        // Informações de usuário específico
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public UserResponse? User { get; set; }

        // Lista de usuários
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<UserResponse>? Users { get; set; }

        // Informações do seguidor
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string FollowerId { get; set; }

        // Informação extra
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Result { get; set; }

        // Informações do estudante
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public StudentById Student { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PostResponse RootPost { get; set; }

        // Lista de Posts
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PostResponse ParentPost { get; set; }

        // Informações do Post
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PostResponse Post { get; set; }

        // Lista de Posts
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PostResponse>? Posts { get; set; }

        // Informações do Chat
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ChatResponse? Chat { get; set; }

        // Informações dos Chats
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ChatResponse>? Chats { get; set; }

        // Informações das Mensagens
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<MessageResponse>? Messages { get; set; }

        // Informações de paginação
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PaginationResponse? Pagination { get; set; }
    }

    public class PaginationResponse
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Total_Elements { get; set; }
        public int Total_Pages { get; set; }
    }
}
