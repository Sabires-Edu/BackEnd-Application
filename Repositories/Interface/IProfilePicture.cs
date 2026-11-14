namespace Sabires_Backend_Application.Repositories.Interface
{
    public interface IProfilePicture
    {
        public Task<string> UploadFile(string token, Guid userid, IFormFile file);
        public Task DeleteFile(string token, string filePath);
    }
}
