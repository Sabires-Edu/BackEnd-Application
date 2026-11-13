using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models
{
    public class UserStudent
    {
        [JsonPropertyName("user_firstname")]
        public required string FirstName { get; set; }

        [JsonPropertyName("user_lastname")]
        public required string LastName { get; set; }

        [JsonPropertyName("user_email")]
        public required string Email { get; set; }

        [JsonPropertyName("user_password")]
        public required string Password { get; set; }

        [JsonPropertyName("user_roleid")]
        public required int RoleId { get; set; }

        [JsonPropertyName("user_statusid")]
        public required int StatusId { get; set; }

        [JsonPropertyName("user_createdat")]
        public required DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("user_updatedat")]
        public required DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("student_birthdate")]
        public required DateTimeOffset BirthDate { get; set; }
    }

    public class GetUsers
    {
        [JsonPropertyName("inicio")]
        public required int Inicio { get; set; }

        [JsonPropertyName("limite")]
        public required int Limite { get; set; }

        [JsonPropertyName("id")]
        public required Guid Id { get; set; }
    }

    public class GetStudent
    {
        [JsonPropertyName("user_userid")]
        public Guid UserId { get; set; }
    }

    public class UserFunction
    {
        [JsonPropertyName("userid")]
        public required Guid UserId { get; set; }

        [JsonPropertyName("firstname")]
        public required string FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public required string LastName { get; set; }

        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("profilepicture")]
        public required string ProfilePicture { get; set; }

        [JsonPropertyName("biography")]
        public required string Biography { get; set; }

        [JsonPropertyName("statusid")]
        public required int StatusId { get; set; }

        [JsonPropertyName("followers")]
        public required int Followers { get; set; }

        [JsonPropertyName("followings")]
        public required int Followings { get; set; }
    }
}
