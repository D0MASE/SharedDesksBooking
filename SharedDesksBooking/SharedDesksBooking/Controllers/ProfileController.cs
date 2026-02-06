using Microsoft.AspNetCore.Mvc;
using SharedDesksBooking.Services;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    // Konstruktoriuje injektuojame servisą
    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserProfile([FromQuery] string firstName, [FromQuery] string lastName)
    {
        var profile = await _profileService.GetUserProfileAsync(firstName, lastName);

        if (profile == null)
        {
            return BadRequest("Vardas ir pavardė yra privalomi.");
        }

        return Ok(profile);
    }
}