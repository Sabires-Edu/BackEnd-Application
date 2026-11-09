using Supabase.Postgrest.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models
{
    [Supabase.Postgrest.Attributes.Table("users")]
    public class User : BaseModel
    {
        [PrimaryKey("userid")]
        public Guid UserId { get; set; }

        // Nome
        [Supabase.Postgrest.Attributes.Column("firstname")]
        public string FirstName { get; set; }

        // Sobrenome
        [Supabase.Postgrest.Attributes.Column("lastname")]
        public string LastName { get; set; }

        // Email
        [Supabase.Postgrest.Attributes.Column("email")]
        public string Email { get; set; }

        // Foto de perfil
        [Supabase.Postgrest.Attributes.Column("profilepicture")]
        public string ProfilePicture { get; set; }

        // Biografia
        [Supabase.Postgrest.Attributes.Column("biography")]
        public string Biography { get; set; }

        // Senha
        [Supabase.Postgrest.Attributes.Column("password")]
        public string Password { get; set; }

        // Papel (estudante ou administrador)
        [Supabase.Postgrest.Attributes.Column("roleid")]
        public int RoleId { get; set; }

        // Status (Ensino Médio, Ensino Superior, Pós-Graduação, etc.)
        [Supabase.Postgrest.Attributes.Column("statusid")]
        public int StatusId { get; set; }

        // Data de criação
        [Supabase.Postgrest.Attributes.Column("createdat")]
        public DateTimeOffset CreatedAt { get; set; }

        // Data de atualização
        [Supabase.Postgrest.Attributes.Column("updatedat")]
        public DateTimeOffset UpdatedAt { get; set; }
    }

    [Supabase.Postgrest.Attributes.Table("publicuser")]
    public class PublicUser : BaseModel
    {
        [PrimaryKey("userid")]
        [JsonPropertyName("userid")]
        public Guid UserId { get; set; }

        // Nome
        [Supabase.Postgrest.Attributes.Column("firstname")]
        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        // Sobrenome
        [Supabase.Postgrest.Attributes.Column("lastname")]
        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        // Email
        [Supabase.Postgrest.Attributes.Column("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        // Foto de perfil
        [Supabase.Postgrest.Attributes.Column("profilepicture")]
        [JsonPropertyName("profilepicture")]
        public string ProfilePicture { get; set; }

        // Biografia
        [Supabase.Postgrest.Attributes.Column("biography")]
        [JsonPropertyName("biography")]
        public string Biography { get; set; }

        // Status (Ensino Médio, Ensino Superior, Pós-Graduação, etc.)
        [Supabase.Postgrest.Attributes.Column("statusid")]
        [JsonPropertyName("statusid")]
        public int StatusId { get; set; }
    }

    [Supabase.Postgrest.Attributes.Table("followers")]
    public class Follower : BaseModel
    {
        [PrimaryKey("followersid")]        
        public int FollowersId { get; set; }
        
        [Supabase.Postgrest.Attributes.Column("followeruserid")]
        public Guid FollowerUserId { get; set; }

        //[Reference(typeof(User))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public User? FollowerUser { get; set; }
        
        [Supabase.Postgrest.Attributes.Column("followinguserid")]
        public Guid FollowingUserId { get; set; }

        //[Reference(typeof(User))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public User? FollowingUser { get; set; }

        [Supabase.Postgrest.Attributes.Column("statusid")]
        public int StatusId { get; set; }
    }
}
