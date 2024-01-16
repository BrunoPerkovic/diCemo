using System.ComponentModel.DataAnnotations;
using SharedBL.Database;

namespace AuthModule.BL.DataModels;

public class User : PostgresBase
{
    [Required] public string Email { get; set; } = null!;
    [Required] public string PasswordHash { get; set; }
    public bool Verified { get; set; }
    public string PhoneNumber { get; set; }
}