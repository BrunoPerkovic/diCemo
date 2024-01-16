using Microsoft.Extensions.Options;

namespace SharedBL.ConfigOptionsSetup;

public class DbOptionsSetup : IConfigureOptions<DbOptions>
{
    private const string SectionName = "Db";
    
    public void Configure(DbOptions options)
    {
        throw new NotImplementedException();
    }
}