using AuthConfigAPI.Data;
using AuthConfigAPI.Interfaces.Repositories;
using AuthConfigAPI.Models;
using AuthConfigAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthConfigAPI.Repositories
{
    public class UserRepository : IUserRegistry
    {
        private readonly AuthenticationDbContext AuthDbContext;
        public UserRepository(AuthenticationDbContext authDbCntext)
        {
            AuthDbContext = authDbCntext;
        }

        public async Task<SignUpRequest> checkUserCredentials(string Email, string password)
        {
            try
            {
                var validateUser = await AuthDbContext.UserData
                  .FirstOrDefaultAsync(userData =>
                 (userData.Email == Email) &&
                 userData.Password == password);

                return validateUser;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
