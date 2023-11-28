using System.ComponentModel.DataAnnotations;
using AuthModule.Utils;
using SharedBL.Database;

namespace AuthModule.BL.DataModels;

public class Address : PostgresBase
{
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Zip { get; set; } = null!;
    public string Country { get; set; } = null!;
    public CountryCode CountryCode { get; set; }
}