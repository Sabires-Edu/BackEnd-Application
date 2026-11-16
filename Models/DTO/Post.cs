namespace Sabires_Backend_Application.Models.DTO
{
    public class PostCreateDTO
    {
        public string Content { get; set; }
        public int? RootPostId { get; set; }        
        public int? ParentPostId { get; set; }
        public List<IFormFile> Images { get; set; } = new();

    }
}
