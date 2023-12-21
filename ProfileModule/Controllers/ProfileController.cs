using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileModule.BL.Intefaces;
using ProfileModule.BL.Models;

namespace ProfileModule.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }
    
    [HttpPost("profile", Name = nameof(PostProfile))]
    public async Task<IActionResult> PostProfile(PostProfileRequest profile)
    {
        var result = await _profileService.PostProfile(profile);
        return CreatedAtRoute("GetProfile", new {id = result.Id}, result);
    }
    
    [HttpGet("profile/{email}", Name = nameof(GetProfile))]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetProfile(string email)
    {
        var result = await _profileService.GetProfile(email);
        return Ok(result);
    }
}