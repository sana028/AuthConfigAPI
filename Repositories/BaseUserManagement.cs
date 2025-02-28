using AuthConfigAPI.Data;
using AuthConfigAPI.Interfaces.Repositories;
using AuthConfigAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthConfigAPI.Repositories
{
    public class BaseUserManagement<T, TUser> : IBaseUserManagement<T, TUser> where TUser : class
    {
        private readonly AuthenticationDbContext AuthDbContext;
        private readonly IMapper Mapper;
        private DbSet<TUser> Users { get; set; }

        public BaseUserManagement(AuthenticationDbContext dbContext, IMapper mapper)
        {
            AuthDbContext = dbContext;
            Users = AuthDbContext.Set<TUser>();
            Mapper = mapper;
        }

        public async Task<List<SignUpRequest>> GetAllUserData()
        {
            try
            {
                var listOfAllUsers = await Users.ToListAsync(); 
                return Mapper.Map<List<SignUpRequest>>(listOfAllUsers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                throw; // Avoid swallowing exceptions
            }
        }

        public async Task<TUser> GetUserData(T userId)
        {
            try
            {
                var userData = await Users.FindAsync(userId);
                if (userData == null)
                {
                    return null;
                }
                return userData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                throw;
            }
        }


    public async Task UpdateUserData(T userId, TUser userData)
        {
            try
            {
                var existedUser = await Users.FindAsync(userId);
                Mapper.Map(existedUser, userData);
                await AuthDbContext.SaveChangesAsync();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task AddNewUserData(TUser user)
        {
            try
            {
                await Users.AddAsync(user);
                await AuthDbContext.SaveChangesAsync();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task DeleteUserData(T userId)
        {
            try
            {
                var item = await Users.FindAsync(userId);
                Users.Remove(item);
                await AuthDbContext.SaveChangesAsync();
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
