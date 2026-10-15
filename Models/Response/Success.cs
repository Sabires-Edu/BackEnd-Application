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
    }
}
