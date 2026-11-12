using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Storefront.Tests;

/// <summary>
/// Boots the real composition root from Program.cs with placeholder configuration,
/// so DI can be exercised without a database or S3 credentials. Nothing here opens
/// a connection: the connection string and S3 keys are only read, never used.
/// </summary>
public class StorefrontFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);
        builder.UseSetting("ConnectionStrings:DefaultConnection", "Host=localhost;Database=storefront_tests;Username=test;Password=test");
        builder.UseSetting("S3:KeyId", "test-key-id");
        builder.UseSetting("S3:SecretKey", "test-secret-key");
        builder.UseSetting("S3:Region", "UsEast1");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Fail fast on missing registrations and captive dependencies, regardless of environment.
        builder.UseDefaultServiceProvider(options =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });
        return base.CreateHost(builder);
    }
}
