using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class AuthenticationContext:IdentityDbContext
    {
        public AuthenticationContext (DbContextOptions options):base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
