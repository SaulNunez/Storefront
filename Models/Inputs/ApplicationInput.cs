using System.ComponentModel.DataAnnotations;

namespace Storefront.Models.Inputs;

public class ApplicationInput
{
    [Required]
    public string ApplicationName { get; set; }
    [Required]
    public string ShortDescription { get; set; }
    [Required]
    public string ApplicationDescription { get; set; }
}