using System.ComponentModel.DataAnnotations;

namespace AuthModule.BL.DataModels;

public class User
{
    [Key] public int Id { get; set; }
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    [Required] public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? ProfilePicture { get; set; }
    public string About { get; set; } = null!;
    public bool Deleted { get; set; }
    
    public Address Address { get; set; } = null!;
}