using System.ComponentModel.DataAnnotations;

namespace AuthModule.BL.Models;

public class UserDto
{
    [Key]
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
}