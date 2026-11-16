namespace Sabires_Backend_Application.Models.Response
{
    public class PostResponse
    {
        public required int PostId { get; set; }
        public required Guid UserId { get; set; }
        public required string Content { get; set; }
        public required int RootPostId { get; set; }
        public required int ParentPostId { get; set; }
        public required List<string> Files { get; set; }
        public required int LikesCount { get; set; }
        public required int CommentsCount { get; set; }
        public required DateTimeOffset CreatedAt { get; set; }
        public required DateTimeOffset UpdatedAt { get; set; }
    }
}
