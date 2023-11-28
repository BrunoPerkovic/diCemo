namespace AuthModule.BL.Models;

public class ProfileCreationResponse
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? FlatNumber { get; set; }
}