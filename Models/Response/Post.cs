using System.Text.Json.Serialization;

namespace Sabires_Backend_Application.Models.Response
{
    public class PostResponse
    {
        public int PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public int RootPostId { get; set; }
        public int ParentPostId { get; set; }
        public List<string> Files { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class CommentResponse
    {
        public required PostResponse Post { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required PostResponse RootPost { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required PostResponse ParentPost { get; set; }

        public required List<PostResponse> Comments { get; set; }
    }
}
