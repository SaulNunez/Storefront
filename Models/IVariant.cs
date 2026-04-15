namespace Storefront.Models;

public interface IVariant
{
    /// <summary>
    /// Identifier for variant.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Where the content is located in blob storage. This path to object will be used to generate URL for clients to download the app.
    /// </summary>
    public string ObjectKeyInStorage { get; set; }
}