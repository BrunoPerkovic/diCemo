using AuthModule.BL.Models.EMail;

namespace AuthModule.BL.Interfaces;

public interface IEmailService
{
    void SendVerificationEmail(string emailRecepient);
}