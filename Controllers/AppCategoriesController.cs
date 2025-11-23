
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Storefront.Services;

[Route("api/[controller]")]
[ApiController]
public class AppCategoriesController(IAppCategoryService categoryService) : Controller
{
    [HttpGet]
    public IActionResult GetAvailableApplicationCategories()
    {
        var categories = categoryService.GetAllAppCategories();

        return Ok(categories);
    }
}