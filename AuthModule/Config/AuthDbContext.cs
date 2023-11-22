using AuthModule.BL.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthModule.Config;

public class AuthDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("DićemoDatabase");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}