using AuthModule.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthModule.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    [HttpPost("send")]
    public IActionResult SendEmail(string emailRecipient)
    {
        _emailService.SendVerificationEmail(emailRecipient);
        return Ok();
    }
}