using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models
{
    public class UserStudent
    {
        [JsonPropertyName("user_firstName")]
        public required string FirstName { get; set; }

        [JsonPropertyName("user_lastName")]
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
}
