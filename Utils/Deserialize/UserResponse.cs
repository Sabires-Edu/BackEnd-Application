using Sabires_Backend_Application.Models;
using Sabires_Backend_Application.Models.Response;

namespace Sabires_Backend_Application.Utils.Deserialize
{
    public class DeserializeUserResponse
    {
        public static UserResponse DeserializeUsers(PublicUser user)
        {
            return new UserResponse
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture,
                Biography = user.Biography,
                StatusId = user.StatusId,
            };
        }
    }
}
