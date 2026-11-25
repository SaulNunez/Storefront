using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefront.Models.Exceptions;
using Storefront.Models.Inputs;
using Storefront.Services;

namespace Storefront.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DevelopersController(ILogger<DevelopersController> logger, IApplicationService applicationService) : ControllerBase
{
    [Authorize(Roles = "Administrator,Developer")]
    [HttpGet("applications")]
    public async Task<IActionResult> DeveloperApplications([FromQuery] int take = 10, [FromQuery] int page = 1)
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
            var data = await applicationService.GetDeveloperApplications(userId!, take, offset);
            return Ok(data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading developer dashboard data");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [Authorize(Roles = "Administrator,Developer")]
    [HttpGet("applications/{applicationId:guid}")]
    public IActionResult DeveloperApplication(Guid applicationId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(applicationService.GetDeveloperApplication(applicationId, userId!));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading application {ApplicationId}", applicationId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [Authorize(Roles = "Administrator,Developer")]
    [HttpPost("applications")]
    public IActionResult PublishApplication([FromBody] ApplicationInput applicationInput)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var application = applicationService.CreateApplication(applicationInput, userId!);
            return CreatedAtAction(nameof(DeveloperApplication), new { applicationId = application.ApplicationId }, application);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing application");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [Authorize(Roles = "Administrator,Developer")]
    [HttpPost("applications/{applicationId:guid}/icon")]
    [RequestSizeLimit(MaxIconBytes + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxIconBytes + 64 * 1024)]
    public async Task<IActionResult> SaveIcon(Guid applicationId, IFormFile? icon)
    {
        if (icon is null || icon.Length == 0)
        {
            return BadRequest("An icon file is required.");
        }

        if (icon.Length > MaxIconBytes)
        {
            return BadRequest($"The icon must be at most {MaxIconBytes / 1024 / 1024} MB.");
        }

        try
        {
            using var fileStream = icon.OpenReadStream();
            if (!await IsPng(fileStream))
            {
                return BadRequest("The icon must be a PNG image.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var application = await applicationService.UploadApplicationIcon(applicationId, userId!, fileStream, "image/png");
            return Ok(application);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading icon for application {ApplicationId}", applicationId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    private const int MaxIconBytes = 2 * 1024 * 1024;
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    /// <summary>
    /// Checks the file's PNG signature (the browser-supplied content type can't be trusted), then rewinds the stream.
    /// </summary>
    private static async Task<bool> IsPng(Stream stream)
    {
        var header = new byte[PngSignature.Length];
        var read = await stream.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false);
        stream.Position = 0;
        return read == header.Length && header.AsSpan().SequenceEqual(PngSignature);
    }
}
