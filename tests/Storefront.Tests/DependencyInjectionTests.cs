using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Storefront.Tests;

public class DependencyInjectionTests(StorefrontFactory factory) : IClassFixture<StorefrontFactory>
{
    private static IEnumerable<Type> FindControllerTypes() =>
        typeof(Program).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsPublic: true }
                        && typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(t))
            .OrderBy(t => t.FullName);

    public static TheoryData<Type> ControllerTypes() => new(FindControllerTypes());

    [Fact]
    public void ServiceProvider_Builds()
    {
        // Building the host runs ValidateOnBuild, which checks every registration's dependencies.
        Assert.NotNull(factory.Services);
    }

    [Fact]
    public void MvcDiscoversEveryControllerInTheAssembly()
    {
        var discovered = factory.Services.GetRequiredService<IActionDescriptorCollectionProvider>()
            .ActionDescriptors.Items
            .OfType<ControllerActionDescriptor>()
            .Select(d => d.ControllerTypeInfo.AsType())
            .ToHashSet();

        Assert.All(FindControllerTypes(), type => Assert.Contains(type, discovered));
    }

    [Theory]
    [MemberData(nameof(ControllerTypes))]
    public void Controller_CanBeConstructedFromServiceProvider(Type controllerType)
    {
        using var scope = factory.Services.CreateScope();

        // Mirrors what MVC's controller activator does per request: throws if any
        // constructor dependency is not registered (e.g. a non-generic ILogger).
        var controller = ActivatorUtilities.CreateInstance(scope.ServiceProvider, controllerType);

        Assert.IsType(controllerType, controller);
    }
}
