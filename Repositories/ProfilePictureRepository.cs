using Sabires_Backend_Application.Repositories.Interface;
using Sabires_Backend_Application.Services;
using Sabires_Backend_Application.Utils.Exceptions;

namespace Sabires_Backend_Application.Repositories
{
    public class ProfilePictureRepository(SupabaseClientService Client) : IProfilePicture
    {
        private const string BucketName = "profile";

        public async Task<string> UploadFile(string token, Guid userid, IFormFile file)
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

            var fileName = $"profile-{Guid.NewGuid()}{extension}";

            var filePath = $"users/{userid}/{fileName}";

            var tempPath = Path.GetTempFileName();

            try
            {
                await using (var stream = File.Create(tempPath))
                {
                    await file.CopyToAsync(stream);
                }

                await client.Storage.From(BucketName).Upload(tempPath, filePath, new Supabase.Storage.FileOptions
                {
                    ContentType = file.ContentType,
                    CacheControl = "3600",
                    Upsert = false
                },
                inferContentType: false);

                return client.Storage.From(BucketName).GetPublicUrl(filePath);
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }
 
        public async Task DeleteFile(string token, string filePath)
        {
            Supabase.Client client = Client.InitCustomClient(token);
            await client.Storage.From(BucketName).Remove(new List<string> { filePath });
        }
    }
}