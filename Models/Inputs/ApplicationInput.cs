using System.ComponentModel.DataAnnotations;

namespace Storefront.Models.Inputs;

public class ApplicationInput
{
    [Required]
    [Display(Name = "Application name")]
    public string ApplicationName { get; set; } = string.Empty;
    [Required]
    [Display(Name = "Short description")]
    public string ShortDescription { get; set; } = string.Empty;
    [Required]
    [Display(Name = "Description")]
    public string ApplicationDescription { get; set; } = string.Empty;
    [Range(1, int.MaxValue, ErrorMessage = "Choose a category.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
}