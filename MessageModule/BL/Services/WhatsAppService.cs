using MessageModule.BL.Interfaces;
using Twilio.Rest.Api.V2010.Account;

namespace MessageModule.BL.Services;

public class WhatsAppService : IWhatsAppService
{
    private string GenerateRandom8DigitCode()
    {
        var length = 8;
        var random = new Random();
        // var cryptoRandom=  System.Security.Cryptography.RandomNumberGenerator.Create();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var result = new string(
            Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        return result;
    }
    public void SendMessageAsync(string recipientNumber)
    {
        var message = MessageResource.Create(from: new Twilio.Types.PhoneNumber("whatsapp:+385917672306"),
            body: GenerateRandom8DigitCode(), to: new Twilio.Types.PhoneNumber($"whatsapp:{recipientNumber}"));
        
        Console.WriteLine(message.Body.ToString());
    }
}