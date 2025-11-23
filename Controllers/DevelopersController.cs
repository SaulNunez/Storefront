using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefront.Services;

[Route("api/[controller]")]
[ApiController]
public class DevelopersController(ILogger<DevelopersController> logger, IApplicationService applicationService) : Controller
{
    [Authorize(Roles = "Administrator,Developer")]
    [HttpGet("applications")]
    public IActionResult DeveloperApplications([FromQuery] int take = 10, [FromQuery] int page = 1)
    {
        if(page < 1)
        {
            return BadRequest("Page number must be greater than 0.");
        }

        if (take < 1 || take > 100)
        {
            return BadRequest("Take parameter must be between 1 and 100.");
        }
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var offset = (page - 1) * take;
            var data = applicationService.GetDeveloperApplications(userId!, take, offset);
            return View(data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading developer dashboard data");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}   