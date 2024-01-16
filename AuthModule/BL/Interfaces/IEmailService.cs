namespace AuthModule.BL.Interfaces;

public interface IEmailService
{
    string SendVerificationEmail(string emailRecipient);
    string ResendVerificationEmail(string emailRecipient);
}