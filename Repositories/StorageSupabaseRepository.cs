using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Utils.Exceptions;

namespace Sabires_Backend_Application.Repositories
{
    public class StorageSupabaseRepository(SupabaseClientService Client) : IStorageSupabase
    {        
        public async Task<string> UploadFile(string token, Guid userid, IFormFile file, string bucket)
        {
            Supabase.Client client = Client.InitCustomClient(token);

            var extension = file.ContentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => throw new BadRequestException(
                    "Formato de imagem não permitido."
                )
            };

            var fileName = $"{bucket}-{Guid.NewGuid()}{extension}";

            var filePath = $"users/{userid}/{fileName}";

            var tempPath = Path.GetTempFileName();

            try
            {
                await using (var stream = File.Create(tempPath))
                {
                    await file.CopyToAsync(stream);
                }

                await client.Storage.From(bucket).Upload(tempPath, filePath, new Supabase.Storage.FileOptions
                {
                    ContentType = file.ContentType,
                    CacheControl = "3600",
                    Upsert = false
                },
                inferContentType: false);

                return client.Storage.From(bucket).GetPublicUrl(filePath);
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }
 
        public async Task DeleteFile(string token, string filePath, string bucket)
        {
            Supabase.Client client = Client.InitCustomClient(token);
            await client.Storage.From(bucket).Remove(new List<string> { filePath });
        }
    }
}