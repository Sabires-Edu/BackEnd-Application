namespace Sabires_Backend_Application.Repositories.Interface
{
    public interface IStorageSupabase
    {
        public Task<string> UploadFile(string token, Guid userid, IFormFile files, string bucket);
        public Task DeleteFile(string token, string filePath, string bucket);
    }
}
