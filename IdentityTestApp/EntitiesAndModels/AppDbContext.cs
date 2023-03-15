using IdentityTestApp.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityTestApp.EntitiesAndModels
{
    public sealed class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<AppUsers> AppUsers { get; set; }

        public DbSet<TestDb> TestDbs { get; set; }
    }
}