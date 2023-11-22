using System.ComponentModel.DataAnnotations;
using SharedBL.Database;

namespace AuthModule.BL.Models;

public class User : PostgresBase
{
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    [Required] public string UserName { get; set; }
    public string PasswordHash { get; set; }
}