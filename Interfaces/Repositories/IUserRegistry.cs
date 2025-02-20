using AuthConfigAPI.Models;

namespace AuthConfigAPI.Interfaces.Repositories
{
    public interface IUserRegistry
    {
        Task<SignUpRequest> checkUserCredentials(string userIdorEmail, string password);

    }
}
