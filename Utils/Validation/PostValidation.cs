using Sabires_Backend_Application.Utils.Exceptions;

namespace Sabires_Backend_Application.Utils.Validation
{
    public class PostValidation
    {
        public void ValidatePostContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new BadRequestException("O conteúdo do post não pode estar vazio.");

            if(content.Length > 500)
                throw new BadRequestException("O conteúdo do post não pode ter mais de 500 caracteres.");
        }

        public void ValidatePostFiles(List<IFormFile> files)
        {
            if (files == null)
                throw new BadRequestException("Os arquivos do post não podem ser nulos.");

            foreach (var file in files)
            {
                if (file.Length > 5 * 1024 * 1024)
                    throw new BadRequestException("O tamanho do arquivo não pode exceder 5MB.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
                    throw new BadRequestException("Formato de arquivo não permitido. Apenas JPG, JPEG, PNG e WEBP são aceitos.");
            }
        }
    }
}
