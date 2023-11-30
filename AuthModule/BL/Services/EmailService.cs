using AuthModule.BL.Interfaces;
using AuthModule.BL.Models.EMail;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace AuthModule.BL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        private string GenerateRandom8DigitCode()
        {
            var length = 8;
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new string(
                Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
            return result;
        }

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendVerificationEmail(string emailRecepient)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(emailRecepient));
            email.Subject = "Code Verification";
            email.Body = new TextPart(TextFormat.Html) { Text = GenerateRandom8DigitCode() };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}