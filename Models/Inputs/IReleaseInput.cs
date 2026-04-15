namespace Storefront.Models.Inputs;

public interface IReleaseInput
{
    public string VersionId { get; set; }
    public string ReleaseNotes { get; set; }
}