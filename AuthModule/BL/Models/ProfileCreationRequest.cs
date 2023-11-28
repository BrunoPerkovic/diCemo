namespace AuthModule.BL.Models;

public class ProfileCreationRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string AboutMe { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
}