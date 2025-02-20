using AuthConfigAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthConfigAPI.Interfaces.Repositories
{
    public interface IBaseUserManagement<T, TUser> where TUser : class
    {
        Task<List<SignUpRequest>> GetAllUserData();

        Task<TUser> GetUserData(T userId);

        Task UpdateUserData(T userId, TUser user);

        Task AddNewUserData(TUser user);

        Task DeleteUserData(T userId);
    }
}
