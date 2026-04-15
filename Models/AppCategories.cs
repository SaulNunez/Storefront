namespace Storefront.Models;

public class AppCategories
{
    /// <summary>
    /// Internal identifier for category. For display purposes, use <see cref="Name"/> instead.
    /// </summary>
    public int Id {get; set;}
    /// <summary>
    /// Display name for category.
    /// </summary>
    public string Name { get; set; }
}