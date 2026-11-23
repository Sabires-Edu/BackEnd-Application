using System.Text.Json.Serialization;

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

        // Data de nascimento
        public required DateOnly BirthDate { get; set; }

        // Total de seguidores
        public required int TotalFollowers { get; set; }

        // Total de usuários que o usuário está seguindo
        public required int TotalFollowing { get; set; }
    }

    public class UserResponse
    {
        // Identificador do usuário
        [JsonPropertyName("userid")]
        public required Guid UserId { get; set; }

        // Nome
        [JsonPropertyName("firstname")]
        public required string FirstName { get; set; }

        // Sobrenome
        [JsonPropertyName("lastname")]
        public required string LastName { get; set; }

        // Email
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        // Foto de perfil

        [JsonPropertyName("profilepicture")]
        public required string ProfilePicture { get; set; }

        // Biografia
        [JsonPropertyName("biography")]
        public required string Biography { get; set; }

        // Status (Ensino Médio, Ensino Superior, Pós-Graduação, etc.)
        [JsonPropertyName("statusid")]
        public required int StatusId { get; set; }

        [JsonPropertyName("followers")]
        public int TotalFollowers { get; set; }

        [JsonPropertyName("following")]
        public int TotalFollowing { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("followerstatusid")]
        public int? FollowerStatus { get; set; }
    }

    public class StudentResponse
    {
        [JsonPropertyName("userid")]
        public Guid UserId { get; set; }

        [JsonPropertyName("birthdate")]
        public DateOnly BirthDate { get; set; }

        [JsonPropertyName("riskid")]
        public int RiskId { get; set; }
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

    public class StudentById {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("firstname")]
        public required string FirstName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("lastname")]
        public required string LastName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("statusid")]
        public required int StatusId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("birthdate")]
        public required DateTimeOffset BirthDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("biography")]
        public required string Biography { get; set; }
    }

    public class MessageUserResponse
    {
        // Identificador do usuário
        [JsonPropertyName("userid")]
        public required Guid UserId { get; set; }

        // Nome
        [JsonPropertyName("firstname")]
        public required string FirstName { get; set; }

        // Sobrenome
        [JsonPropertyName("lastname")]
        public required string LastName { get; set; }

        // Foto de perfil
        [JsonPropertyName("profilepicture")]
        public required string ProfilePicture { get; set; }
    }

}
