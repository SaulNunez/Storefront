using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Storefront.Models;
using Storefront.Models.Exceptions;
using Storefront.Services;

namespace Storefront.Controllers;

public class ApplicationController(ILogger<ApplicationController> logger, IApplicationService applicationService) : Controller
{
    [HttpGet("{applicationId:guid}")]
    public IActionResult Details(Guid applicationId)
    {
        try
        {
            var applicationInformation = applicationService.GetApplication(applicationId);
            return View(applicationInformation);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading application {ApplicationId}", applicationId);
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}