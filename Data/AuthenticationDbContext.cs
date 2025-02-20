using AuthConfigAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthConfigAPI.Data
{
    public class AuthenticationDbContext:DbContext
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options) { }

        public DbSet<SignUpRequest> UserData { get; set; }

        public DbSet<TemporarySignUpData> TemporarySignUpData { get;set; }

    }
}
