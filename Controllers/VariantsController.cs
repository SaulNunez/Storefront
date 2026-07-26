using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefront.Models.Enums;
using Storefront.Models.Exceptions;
using Storefront.Models.Inputs;
using Storefront.Services;

namespace Storefront.Controllers;

[ApiController]
[Route("api/releases/{releaseId:guid}/[controller]")]
public class VariantsController(ILogger<VariantsController> logger, IReleaseService releaseService) : ControllerBase
{
    [HttpGet("{variantId:guid}")]
    public async Task<IActionResult> GetVariant(Guid variantId)
    {
        try
        {
            var variant = await releaseService.GetVariantAsync(variantId);
            return Ok(variant);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching variant {VariantId}", variantId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [Authorize(Roles = "Administrator,Developer")]
    [HttpPost]
    public async Task<IActionResult> CreateVariant(Guid releaseId, [FromBody] VariantInput input)
    {
        try
        {
            var variant = await releaseService.CreateVariantAsync(releaseId, input);
            return CreatedAtAction(nameof(GetVariant), new { releaseId, variantId = variant.Id }, variant);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating variant for release {ReleaseId}", releaseId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [Authorize(Roles = "Administrator,Developer")]
    [HttpDelete("{variantId:guid}")]
    public async Task<IActionResult> DeleteVariant(Guid variantId)
    {
        try
        {
            var deleted = await releaseService.DeleteVariantAsync(variantId);
            return deleted ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting variant {VariantId}", variantId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("select")]
    public async Task<IActionResult> MatchVariant(Guid releaseId, [FromQuery] CpuArchitecture cpu, [FromQuery] AndroidScreenDensity? density)
    {
        try
        {
            var variant = await releaseService.SelectBestVariantAsync(releaseId, cpu, density);
            return variant != null ? Ok(variant) : NotFound("No compatible variant found for specified hardware specifications.");
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error selecting variant for release {ReleaseId}", releaseId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
