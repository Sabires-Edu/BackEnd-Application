using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models.Response
{
    public class FollowerResponse
    {
        [JsonPropertyName("followersid")]
        public required int FollowersId { get; set; }

        [JsonPropertyName("followeruserid")]
        public required Guid FollowerUserId { get; set; }

        [JsonPropertyName("followinguserid")]
        public required Guid FollowingUserId { get; set; }

        [JsonPropertyName("statusid")]
        public required int StatusId { get; set; }

        [JsonPropertyName("publicuser")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PublicUser? PublicUser { get; set; }
    }
}
