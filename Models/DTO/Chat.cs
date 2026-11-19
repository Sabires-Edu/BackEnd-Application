namespace Sabires_Backend_Application.Models.DTO
{
    public class CreateChatDTO
    {
        public required int ChatTypeId { get; set; }
        public string ChatName { get; set; }
        public string? ChatDescription { get; set; }
        public List<string> MembersId { get; set; }
    }
}
