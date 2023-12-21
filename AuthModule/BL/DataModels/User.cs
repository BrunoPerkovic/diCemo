using System.ComponentModel.DataAnnotations;
using SharedBL.Database;

namespace AuthModule.BL.DataModels;

public class User : PostgresBase
{
    [Required] public string Email { get; set; } = null!;
    public string PasswordHash { get; set; }
    public string VerificationCode { get; set; } = null!;
}