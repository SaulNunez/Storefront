using Storefront.Models;
using Storefront.Models.DAO;
using Storefront.Models.Exceptions;
using Storefront.Services.Converters;

public class ApplicationService(StorefrontDbContext dbContext)
{
    public ApplicationDao GetApplication(Guid id)
    {
        var application = dbContext.Applications.Find(id) ?? throw new NotFoundException($"Application with ID {id} not found!");
        return application.ToDao();
    }

    public List<ApplicationDao> GetLatestApplications(int maxLength = 10)
    {
        var latestApplications = dbContext.Applications
        .OrderByDescending(a => a.CreatedAt)
        .Take(maxLength)
        .ToList();

        return [.. latestApplications.Select(a => a.ToDao())];
    }
    
    public List<ApplicationDao> GetMostPopularApplications(int maxLength = 10)
    {
        //Change to use download count
        var latestApplications = dbContext.Applications
        .Take(maxLength)
        .ToList();

        return [.. latestApplications.Select(a => a.ToDao())];
    }
}