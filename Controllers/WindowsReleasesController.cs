using Microsoft.AspNetCore.Mvc;
using Storefront.Models;
using Storefront.Models.Inputs;
using Storefront.Services;

namespace Storefront.Controllers;

[ApiController]
[Route("api/developers/applications/{applicationId}/[controller]")]
public class WindowsReleasesController(ILogger logger, IReleaseService applicationService): Controller
{
    [HttpGet]
    public IActionResult GetAllWindowsReleases(Guid applicationId)
    {
        try
        {
            var releases = applicationService.GetAllApplicationsWindowsRelease(applicationId);
            return Ok(releases);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing application");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    } 

    [HttpGet("{releaseId}")]
    public IActionResult GetWindowsRelease(Guid releaseId)
    {
        try
        {
            var release = applicationService.GetWindowsRelease(releaseId);
            return Ok(release);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing application");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public IActionResult CreateWindowsRelease([FromBody]WindowsReleaseInput windowsReleaseInput, Guid applicationId)
    {
        try
        {
            var release = applicationService.CreateWindowsRelease(applicationId, windowsReleaseInput);
            return Created();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing application");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}