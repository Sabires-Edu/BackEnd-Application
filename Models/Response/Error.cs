namespace Sabires_Backend_Application.Models.Response
{
    public class ErrorResponse
    {
        // Sucesso da requisição
        public required bool Success { get; set; }

        // Descrição breve do erro
        public required string Title { get; set; }

        // Detalhes do erro
        public required string Detail { get; set; }
        
        // Código de status HTTP
        public required int Status { get; set; }
    }
}
