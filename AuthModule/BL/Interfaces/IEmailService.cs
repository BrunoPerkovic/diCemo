using AuthModule.BL.Models.EMail;

namespace AuthModule.BL.Interfaces;

public interface IEmailService
{
    string SendVerificationEmail(string emailRecipient);
}