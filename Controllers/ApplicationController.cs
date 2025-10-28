using Microsoft.AspNetCore.Mvc;

namespace Storefront.Controllers;

public class ApplicationController : Controller
{
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