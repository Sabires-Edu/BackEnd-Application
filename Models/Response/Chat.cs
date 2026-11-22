using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models.Response
{
    public class ChatResponse
    {
        [JsonPropertyName("chatid")]
        public int ChatId { get; set; }

        [JsonPropertyName("chattypeid")]
        public int ChatTypeId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("ownerid")]
        public Guid? OwnerId { get; set; }

        [JsonPropertyName("chatname")]
        public string ChatName { get; set; }

        [JsonPropertyName("chatdescription")]
        public string ChatDescription { get; set; }

        [JsonPropertyName("createdat")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updateat")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<UserResponse>? Members { get; set; }
    }

    public class ChatMembersResponse
    {
        [JsonPropertyName("chatmemberid")]
        public int ChatMemberId { get; set; }

        [JsonPropertyName("chatid")]
        public int ChatId { get; set; }

        [JsonPropertyName("userid")]
        public Guid UserId { get; set; }

        [JsonPropertyName("chat")]
        public ChatJoinResponse Chat { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("publicuser")]
        public UserResponse? Members { get; set; }
    }

    public class ChatJoinResponse
    {
        [JsonPropertyName("chatid")]
        public int ChatId { get; set; }

        [JsonPropertyName("ownerid")]
        public Guid OwnerId { get; set; }

        [JsonPropertyName("chatname")]
        public string ChatName { get; set; }

        [JsonPropertyName("chatdescription")]
        public string ChatDescription { get; set; }

        [JsonPropertyName("chattypeid")]
        public int ChatTypeId { get; set; }

        [JsonPropertyName("createdat")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updatedat")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("deletedat")]
        public DateTimeOffset? DeletedAt { get; set; }
    }

    public class MessageResponse
    {
        [JsonPropertyName("messageid")]
        public int MessageId { get; set; }

        [JsonPropertyName("userid")]
        public Guid UserId { get; set; }

        [JsonPropertyName("chatid")]
        public int ChatId { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("sentat")]
        public DateTimeOffset SentAt { get; set; }

        [JsonPropertyName("updatedat")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("deletedat")]
        public DateTimeOffset? DeletedAt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("publicuser")]
        public MessageUserResponse? PublicUser { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("chat")]
        public MessageChatResponse? Chat { get; set; }
    }

    public class MessageChatResponse
    {
        [JsonPropertyName("chatid")]
        public int ChatId { get; set; }

        [JsonPropertyName("chatname")]
        public string ChatName { get; set; }
    }
}
