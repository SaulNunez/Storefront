using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Storefront.Models;
using Storefront.Services;

namespace Storefront.Controllers;

public class HomeController(ILogger<HomeController> logger, IApplicationService applicationService) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    { 
        try
        {
            var data = applicationService.GetHomeScreenData();
            return View(data);
        } 
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading home screen data");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
