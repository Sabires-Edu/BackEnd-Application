using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models.Response
{
    public class ChatResponse
    {
        [JsonPropertyName("chatid")]
        public int ChatId { get; set; }

        [JsonPropertyName("chattypeid")]
        public int ChatTypeId { get; set; }

        [JsonPropertyName("ownerid")]
        public Guid OwnerId { get; set; }

        [JsonPropertyName("chatname")]
        public string ChatName { get; set; }

        [JsonPropertyName("chatdescription")]
        public string ChatDescription { get; set; }

        [JsonPropertyName("createdat")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updateat")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
