using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefront.Models.Exceptions;
using Storefront.Models.Inputs;
using Storefront.Services;

namespace Storefront.Controllers;

[ApiController]
[Route("api/Application/{applicationId}/WindowsRelease/{releaseId}/[controller]")]
public class WindowsVariantController(IApplicationService applicationService, ILogger<HomeController> logger): ControllerBase
{
    [Authorize(Roles = "Administrator,Developer")]
    [HttpPost]
    public IActionResult CreateWindowsVariant(Guid applicationId, Guid releaseId, [FromBody] WindowsVariantInput variantName)
    {
        var variantId = applicationService.UploadWindowsAsync(applicationId, releaseId, variantName.TargetPlatform, variantName.ClientFileName);
        return CreatedAtAction(nameof(GetWindowsVariant), new { applicationId = applicationId, releaseId = releaseId, variantId = variantId });
    }

    [HttpGet("{variantId}")]
    public IActionResult GetWindowsVariant(Guid variantId)
    {
        try
        {
            var variantInformation = applicationService.GetWindowsVariant(variantId);
            return Ok(variantInformation);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading Windows variant data");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [Authorize(Roles = "Administrator,Developer")]
    [HttpPost("{variantId}/Upload")]
    public async Task<IActionResult> LinkForUploadingWindowsVariant(Guid variantId)
    {
        try
        {
            var uploadLink = await applicationService.CreateWindowsVariantUploadLink(variantId);
            return new CreatedResult(uploadLink, new { UploadUrl = uploadLink });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating upload link for Windows variant");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [Authorize(Roles = "Administrator,Developer")]
    [HttpDelete("{variantId}")]
    public IActionResult DeleteWindowsVariant(Guid variantId)
    {   
        try
        {
            applicationService.DeleteWindowsVariant(variantId);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting Windows variant data");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}