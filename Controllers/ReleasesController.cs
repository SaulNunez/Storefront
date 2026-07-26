using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefront.Models.Enums;
using Storefront.Models.Exceptions;
using Storefront.Models.Inputs;
using Storefront.Services;

namespace Storefront.Controllers;

[ApiController]
[Route("api/applications/{applicationId:guid}/[controller]")]
public class ReleasesController(ILogger<ReleasesController> logger, IReleaseService releaseService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetReleases(Guid applicationId, [FromQuery] TargetPlatform? platform, [FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        try
        {
            var releases = await releaseService.GetReleasesForAppAsync(applicationId, platform, skip, take);
            return Ok(releases);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching releases for application {ApplicationId}", applicationId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{releaseId:guid}")]
    public async Task<IActionResult> GetRelease(Guid releaseId)
    {
        try
        {
            var release = await releaseService.GetReleaseAsync(releaseId);
            return Ok(release);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching release {ReleaseId}", releaseId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [Authorize(Roles = "Administrator,Developer")]
    [HttpPost]
    public async Task<IActionResult> CreateRelease(Guid applicationId, [FromBody] ReleaseInput input)
    {
        try
        {
            var release = await releaseService.CreateReleaseAsync(applicationId, input);
            return CreatedAtAction(nameof(GetRelease), new { applicationId, releaseId = release.Id }, release);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating release for application {ApplicationId}", applicationId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [Authorize(Roles = "Administrator,Developer")]
    [HttpDelete("{releaseId:guid}")]
    public async Task<IActionResult> DeleteRelease(Guid releaseId)
    {
        try
        {
            var deleted = await releaseService.DeleteReleaseAsync(releaseId);
            return deleted ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting release {ReleaseId}", releaseId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
