using System.ComponentModel.DataAnnotations;
using SharedBL.Database;

namespace AuthModule.BL.DataModels;

public class User : PostgresBase
{
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    [Required] public string UserName { get; set; }
    public string PasswordHash { get; set; }
    [Required]public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
    public string? ProfilePicture { get; set; }
    public string? About { get; set; } = null!;
    public bool Deleted { get; set; } = false;
    
    public Address Address { get; set; } = null!;
}