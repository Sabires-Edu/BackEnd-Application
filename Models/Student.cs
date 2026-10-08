using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sabires_Backend_Application.Models
{
    [Supabase.Postgrest.Attributes.Table("users")]
    public class Student : BaseModel
    {
        [ForeignKey("userid")]
        public required Guid UserId { get; set; }
        public required DateTimeOffset BirthDate { get; set; }
        public required int RiskId { get; set; }
    }
}
