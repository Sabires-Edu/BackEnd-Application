using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models
{

    [Supabase.Postgrest.Attributes.Table("student")]
    public class Student : BaseModel
    {
        [Supabase.Postgrest.Attributes.Column("userid")]
        public Guid UserId { get; set; }

        ////[Reference(typeof(User))]
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public PublicUser? User { get; set; }

        // Data de nascimento
        [Supabase.Postgrest.Attributes.Column("birthdate")]
        public DateTimeOffset BirthDate { get; set; }

        // ID de risco
        [Supabase.Postgrest.Attributes.Column("riskid")]
        public int RiskId { get; set; }
    }
}
