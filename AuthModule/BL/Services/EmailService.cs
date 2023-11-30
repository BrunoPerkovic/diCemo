using AuthModule.BL.Interfaces;

namespace AuthModule.BL.Services;

public class EmailService : IEmailService
{
    private string GenerateEmailConfirmationToken(int size = 8)
    {
        var random = new Random();
        var bytes = new byte[32];
        random.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}