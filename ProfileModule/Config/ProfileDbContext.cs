using Microsoft.EntityFrameworkCore;
using ProfileModule.BL.DataModels;

namespace ProfileModule.Config;

public class ProfileDbContext : DbContext
{
    public DbSet<Profile> Profiles { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    
    public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options)
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