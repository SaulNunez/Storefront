using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Storefront.Tests;

/// <summary>
/// The developer portal calls /api with the Identity cookie, so a signed-out request there must get a
/// status code it can act on, while the Razor pages keep redirecting to the login page.
/// ASP.NET Core 10 does this for [ApiController] endpoints without any cookie configuration; these tests
/// keep it that way. None of these requests reach the database: authorization rejects them first.
/// </summary>
public class ApiAuthenticationTests(StorefrontFactory factory) : IClassFixture<StorefrontFactory>
{
    private HttpClient CreateClient() => factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
        BaseAddress = new Uri("https://localhost")
    });

    [Theory]
    [InlineData("GET", "/api/developers/applications")]
    [InlineData("GET", "/api/developers/applications/6f0f7f5e-2f5b-4c1e-9a43-3d1b8a6b1c2d")]
    [InlineData("POST", "/api/developers/applications")]
    [InlineData("POST", "/api/developers/applications/6f0f7f5e-2f5b-4c1e-9a43-3d1b8a6b1c2d/icon")]
    public async Task SignedOutApiRequest_Returns401(string method, string url)
    {
        using var client = CreateClient();

        var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod(method), url));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignedOutPageRequest_RedirectsToLogin()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/Submit");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location?.ToString());
    }
}
