using Microsoft.AspNetCore.Mvc;
using Storefront.Services;

namespace Storefront.Controllers;

public class ApplicationController : Controller
{
    [HttpGet("{applicationId:guid}")]
    public IActionResult Details([FromServices] IApplicationService applicationService, Guid applicationId)
    {
        var applicationInformation = applicationService.GetApplication(applicationId);

        if(applicationInformation == null)
        {
            return NotFound();
        }

        return View(applicationInformation);
    }
}