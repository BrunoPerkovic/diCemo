namespace SharedBL.Profile;

public class ProfileSearchRequest
{
    public int? Id { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CountryCode { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public int Age { get; set; }
}