using Microsoft.IdentityModel.Tokens;
using Sabires_Backend_Application.Models.DTO;
using Sabires_Backend_Application.Models.Response;
using Sabires_Backend_Application.Repositories.Interface;
using System.Text.RegularExpressions;

namespace Sabires_Backend_Application.Utils.Validation
{
    public class UserValidation
    {
        public static readonly Regex NameRegex = new Regex(@"[^\p{L}\p{N} ]");

        public static readonly Regex EmailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        private static readonly Regex SpecialCharRegex = new Regex(@"[^\p{L}\p{Nd}_]");
        private static readonly Regex UpperCaseRegex = new Regex(@"[A-Z]");
        private static readonly Regex LowerCaseRegex = new Regex(@"[a-z]");
        private static readonly Regex NumberRegex = new Regex(@"[0-9]");

        public static void ValidateUserInput(CreateUserDTO user, IUserRepository repository)
        {
            ValidateFirstName(user.FirstName);
            ValidateLastName(user.LastName);
            ValidateEmail(repository, user.Email);
            ValidatePassword(user.Password);
            ValidateBirthDate(user.BirthDate);
            ValidateStatus(user.StatusId);
        }

        public static void ValidateFirstName(string FirstName, string currentName = "")
        {
            if (FirstName.IsNullOrEmpty())
                throw new BadHttpRequestException("O primeiro nome não pode ser vazio. Por favor insira um primeiro nome válido.");

            if (FirstName.Length < 2 || FirstName.Length > 50)
                throw new BadHttpRequestException("O primeiro nome deve ter entre 2 e 50 caracteres. Por favor insira um primeiro nome com um comprimento válido.");

            if (NameRegex.IsMatch(FirstName))
                throw new BadHttpRequestException("O primeiro nome contém caracteres inválidos. Por favor insira apenas letras.");

            if (FirstName == currentName)
                throw new BadHttpRequestException("O novo primeiro nome não pode ser igual ao nome atual. Por favor altere para um nome diferente.");
        }

        public static void ValidateLastName(string LastName, string currentName = "")
        {
            if (LastName.IsNullOrEmpty())
                throw new BadHttpRequestException("O sobrenome não pode ser vazio. Por favor insira um sobrenome válido.");

            if (LastName.Length < 2 || LastName.Length > 50)
                throw new BadHttpRequestException("O sobrenome deve ter entre 2 e 50 caracteres. Por favor insira um sobrenome com um comprimento válido.");

            if (NameRegex.IsMatch(LastName))
                throw new BadHttpRequestException("O sobrenome contém caracteres inválidos. Por favor insira apenas letras.");

            if (LastName == currentName)
                throw new BadHttpRequestException("O novo sobrenome não pode ser igual ao sobrenome atual. Por favor altere para um sobrenome diferente.");
        }

        public static void ValidateEmail(IUserRepository repository, string Email, string currentEmail = "")
        {
            if (Email.IsNullOrEmpty())
                throw new BadHttpRequestException("O email não pode ser vazio. Por favor insira um email válido.");

            if (!EmailRegex.IsMatch(Email))
                throw new BadHttpRequestException("O email não é válido. Por favor insira um email válido.");

            if (Email == currentEmail)
                throw new BadHttpRequestException("O novo email não pode ser igual ao email atual. Por favor altere para um email diferente.");

            UserByEmailResponse user = repository.GetUserByEmail(Email).Result;

            if (!user.IsNull)
                throw new BadHttpRequestException("O email já está em uso. Por favor insira um email diferente.");
        }

        public static void ValidatePassword(string Password, string currentPassword = "")
        {
            if (Password == currentPassword)
                throw new BadHttpRequestException("A nova senha não pode ser igual à senha atual. Por favor altere para uma senha diferente.");

            if (Password.IsNullOrEmpty())
                throw new BadHttpRequestException("A senha não pode ser vazia. Por favor insira uma senha válida.");

            if (Password.Length < 8)
                throw new BadHttpRequestException("A senha deve ter pelo menos 8 caracteres. Por favor insira uma senha com um comprimento válido.");

            if (Password.Length > 50)
                throw new BadHttpRequestException("A senha deve ter no máximo 50 caracteres. Por favor insira uma senha com um comprimento válido.");

            if (!UpperCaseRegex.IsMatch(Password))
                throw new BadHttpRequestException("A senha deve conter pelo menos uma letra maiúscula. Por favor insira uma senha válida.");

            if (!LowerCaseRegex.IsMatch(Password))
                throw new BadHttpRequestException("A senha deve conter pelo menos uma letra minúscula. Por favor insira uma senha válida.");

            if (!NumberRegex.IsMatch(Password))
                throw new BadHttpRequestException("A senha deve conter pelo menos um número. Por favor insira uma senha válida.");

            if (!SpecialCharRegex.IsMatch(Password))
                throw new BadHttpRequestException("A senha deve conter pelo menos um caractere especial. Por favor insira uma senha válida.");
        }

        public static void ValidateBirthDate(DateTimeOffset? BirthDate, DateTimeOffset? currentBirthDate = null)
        {
            if (!BirthDate.HasValue)
                throw new BadHttpRequestException("A data de nascimento não pode ser vazia. Por favor insira uma data de nascimento válida.");

            if (BirthDate == currentBirthDate)
                throw new BadHttpRequestException("A nova data de nascimento não pode ser igual à data de nascimento atual. Por favor altere para uma data de nascimento diferente.");

            DateTimeOffset currentDate = DateTimeOffset.Now;

            if (BirthDate > currentDate)
                throw new BadHttpRequestException("A data de nascimento não pode ser no futuro. Por favor insira uma data de nascimento válida.");

            int age = currentDate.Year - BirthDate.Value.Year;

            if (BirthDate.Value.Month < currentDate.Month || (BirthDate.Value.Month == currentDate.Month && BirthDate.Value.Day < currentDate.Day))
                age--;

            if (age < 13)
                throw new BadHttpRequestException("O usuário deve ter pelo menos 13 anos. Por favor insira uma data de nascimento válida.");
        }

        public static void ValidateStatus(int StatusId, int currentStatusId = -1)
        {
            List<int> options = new List<int> { 1, 2, 3, 4, 5, 6 };

            if (!options.Contains(StatusId))
                throw new BadHttpRequestException("O status fornecido é inválido. Por favor insira um status válido.");

            if (StatusId == currentStatusId)
                throw new BadHttpRequestException("O novo status não pode ser igual ao status atual. Por favor altere para um status diferente.");
        }

    }
}
