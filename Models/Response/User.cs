namespace Sabires_Backend_Application.Models.Response
{
    public class MeResponse
    {
        // Identificador do usuário
        public required Guid UserId { get; set; }

        // Nome
        public required string FirstName { get; set; }

        // Sobrenome
        public required string LastName { get; set; }

        // Email
        public required string Email { get; set; }

        // Foto de perfil
        public required string ProfilePicture { get; set; }

        // Biografia
        public required string Biography { get; set; }

        // Status (Ensino Médio, Ensino Superior, Pós-Graduação, etc.)
        public required int StatusId { get; set; }

        // Data de criação
        public required DateTimeOffset CreatedAt { get; set; }

        // Data de atualização
        public required DateTimeOffset UpdatedAt { get; set; }
    }

    public class UserResponse
    {
        // Identificador do usuário
        public required Guid UserId { get; set; }

        // Nome
        public required string FirstName { get; set; }

        // Sobrenome
        public required string LastName { get; set; }

        // Email
        public required string Email { get; set; }

        // Foto de perfil
        public required string ProfilePicture { get; set; }

        // Biografia
        public required string Biography { get; set; }

        // Status (Ensino Médio, Ensino Superior, Pós-Graduação, etc.)
        public required int StatusId { get; set; }
    }

    public class UserByEmailResponse
    {
        // Informações do usuário
        public PublicUser? User { get; set; }

        // Indica se o usuário é nulo
        public required bool IsNull { get; set; }
    }

    public class PrivateUserByEmailResponse
    {
        // Informações do usuário
        public User? User { get; set; }

        // Indica se o usuário é nulo
        public required bool IsNull { get; set; }
    }
}
