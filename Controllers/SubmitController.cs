using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

public class SubmitController() : Controller
{
    public async Task<IActionResult> UploadAndroidRelease()
    {
        if (!Request.ContentType?.StartsWith("multipart/form-data") ?? true)
        {
            return BadRequest("The request does not contain valid multipart form data.");
        }

        var boundary = HeaderUtilities.RemoveQuotes(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse(Request.ContentType).Boundary).Value;
        if (string.IsNullOrWhiteSpace(boundary))
        {
            return BadRequest("Missing boundary in multipart form data.");
        }

        var cancellationToken = HttpContext.RequestAborted;
        var filePath = await _fileManager.SaveViaMultipartReaderAsync(boundary, Request.Body, cancellationToken);
        return Ok("Saved file at " + filePath);
    }
    
    public IActionResult SubmitApplication([FromServices] IApplicationService applicationService, ApplicationInput applicationInput)
    {
        try
        {
            applicationService.CreateApplication(applicationInput);
            return Created();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public IActionResult MacRelease(Guid applicationId)
    {
        return View();
    }

    [HttpPost]
    public IActionResult PostMacRelease(Guid applicationId)
    {
        return View();
    }

    public IActionResult WindowsRelease(Guid applicationId)
    {
        return View();
    }

    [HttpPost]
    public IActionResult PostWindowsRelease(Guid applicationId)
    {
        return View();
    }

    public IActionResult AndroidRelease(Guid applicationId)
    {
        return View();
    }

    [HttpPost]
    public IActionResult PostAndroidRelease(Guid applicationId)
    {
        return View();
    }
}