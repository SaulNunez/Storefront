using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Storefront.Models;
using Storefront.Models.DAO;
using Storefront.Models.Inputs;
using Storefront.Services;

namespace Storefront.Controllers;

[Authorize(Roles = "Administrator,Developer")]
public class SubmitController(ILogger<SubmitController> logger, IApplicationService applicationService, IAppCategoryService categoryService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        PopulateCategories();
        return View(new ApplicationInput());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(ApplicationInput applicationInput)
    {
        if (!ModelState.IsValid)
        {
            PopulateCategories();
            return View(applicationInput);
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var application = applicationService.CreateApplication(applicationInput, userId!);
            return RedirectToAction(nameof(ApplicationController.Details), "Application", new { applicationId = application.ApplicationId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error submitting application");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // Release and variant pages are still static mockups: binary upload to object storage is not implemented yet.
    [HttpGet]
    public IActionResult AndroidRelease() => View();

    [HttpGet]
    public IActionResult MacRelease() => View();

    [HttpGet]
    public IActionResult MacVariants() => View();

    [HttpGet]
    public IActionResult WindowsRelease() => View();

    private void PopulateCategories()
    {
        ViewData["Categories"] = new SelectList(categoryService.GetAllAppCategories(), nameof(AppCategoriesDao.Id), nameof(AppCategoriesDao.Name));
    }
}