using AuthModule.BL.Interfaces;
using AuthModule.BL.Models.EMail;
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
    public IActionResult SendEmail(EMailDto request)
    {
        _emailService.SendEmail(request);
        return Ok();
    }
}