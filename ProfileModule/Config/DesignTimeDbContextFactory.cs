using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProfileModule.Config;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProfileDbContext>
{
    public ProfileDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = configuration.GetConnectionString("DićemoDatabase");

        var optionsBuilder = new DbContextOptionsBuilder<ProfileDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ProfileDbContext(optionsBuilder.Options);
    }
}