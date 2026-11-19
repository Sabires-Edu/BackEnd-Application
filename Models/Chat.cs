using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models
{
    [Supabase.Postgrest.Attributes.Table("chat")]
    public class Chat : BaseModel
    {
        [PrimaryKey("chatid")]
        public int ChatId { get; set; }

        // ID do tipo de chat
        [Supabase.Postgrest.Attributes.Column("chattypeid")]
        public int ChatTypeId { get; set; }

        // ID do proprietário do grupo
        [Supabase.Postgrest.Attributes.Column("ownerid")]
        public Guid OwnerId { get; set; }

        // Nome do grupo
        [Supabase.Postgrest.Attributes.Column("chatname")]
        public string ChatName { get; set; }

        // Descrição do grupo
        [Supabase.Postgrest.Attributes.Column("chatdescription")]
        public string ChatDescription { get; set; }

        // Data de criação do post
        [Supabase.Postgrest.Attributes.Column("createdat")]
        public DateTimeOffset CreatedAt { get; set; }

        // Data de atualização do post
        [Supabase.Postgrest.Attributes.Column("updatedat")]
        public DateTimeOffset UpdatedAt { get; set; }

        // Data de exclusão do post
        [Supabase.Postgrest.Attributes.Column("deletedat")]
        public DateTimeOffset DeletedAt { get; set; }
    }

    public class ChatConfig
    {
        [JsonPropertyName("chattypeid_param")]
        public int ChatTypeId { get; set; }

        [JsonPropertyName("ownerid_param")]
        public Guid OwnerId { get; set; }

        [JsonPropertyName("chatname_param")]
        public string ChatName { get; set; }

        [JsonPropertyName("chatdescription_param")]
        public string ChatDescription { get; set; }

        [JsonPropertyName("createdat_param")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updatedat_param")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("membersid_param")]
        public List<Guid> MembersId { get; set; }
    }
}
