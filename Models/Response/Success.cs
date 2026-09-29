using System.Reflection;

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
        public string? JWT { get; set; }

        // Informações do usuário logado
        public MeResponse? Me { get; set; }

        // Informações de usuário específico
        public UserResponse? User { get; set; }

        // Lista de usuários
        public List<UserResponse>? Users { get; set; }
    }
}
