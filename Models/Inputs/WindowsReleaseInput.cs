namespace Storefront.Models.Inputs;
public class WindowsReleaseInput: IReleaseInput
{
    public required string VersionId { get; set; }
    public required string ReleaseNotes { get; set; }
}