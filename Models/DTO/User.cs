using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models.DTO
{
    public class CreateUserDTO
    {
        // Nome
        public required string FirstName  { get; set; }

        // Sobrenome
        public required string LastName { get; set; }

        // Email
        public required string Email { get; set; }

        // Senha
        public required string Password { get; set;  }

        // Data de nascimento
        public required DateTimeOffset BirthDate { get; set; }

        // Status (Ensino Médio, Ensino Superior, Pós-Graduação, etc.)
        public required int StatusId { get; set; }     
    }

    public class LoginUserDTO
    {
        // Email
        public required string Email { get; set; }
        
        // Senha
        public required string Password { get; set; }
    }

    public class ChangeFollowStatusDTO
    {                
        public required int Status { get; set; }
    }

    public class UpdateUserDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public int? StatusId { get; set; }
        public string? Biography { get; set; }
    }

    public class UpdatePasswordDTO
    {
        public required string CurrentPassword { get; set; }
        public required string ConfirmNewPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
