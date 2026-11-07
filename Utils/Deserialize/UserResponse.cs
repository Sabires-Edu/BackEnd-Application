using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.Response;
using OneOf;

namespace Sabires_Backend_Application.Utils.Deserialize
{
    public class DeserializeUserResponse
    {
        public static UserResponse DeserializeUsers(OneOf<User, PublicUser> user)
        {
            Console.WriteLine("Teste");

            return user.Match(
                u => new UserResponse
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    ProfilePicture = u.ProfilePicture,
                    Biography = u.Biography,
                    StatusId = u.StatusId,
                },
                pu => new UserResponse
                {
                    UserId = pu.UserId,
                    FirstName = pu.FirstName,
                    LastName = pu.LastName,
                    Email = pu.Email,
                    ProfilePicture = pu.ProfilePicture,
                    Biography = pu.Biography,
                    StatusId = pu.StatusId,
                }
            );
        }
    }
}