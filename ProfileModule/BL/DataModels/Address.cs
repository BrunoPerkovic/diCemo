using ProfileModule.Utils;
using SharedBL.Database;

namespace ProfileModule.BL.DataModels;

public class Address : PostgresBase
{
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Zip { get; set; } = null!;
    public string Country { get; set; } = null!;
    public CountryCode CountryCode { get; set; }
}