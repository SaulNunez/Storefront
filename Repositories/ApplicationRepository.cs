using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Storefront.Models;

namespace Storefront.Repositories;

public interface IApplicationRepository
{
    Application CreateApplication(Application application);
    Application? GetApplication(Guid id);
    IQueryable<Application> GetLatestApplications(int maxLength = 10);
    IQueryable<Application> GetMostPopularApplications(int maxLength = 10);
    IQueryable<Application> GetApplicationsByDeveloper(string userId, int take = 10, int skip = 0);
}

public class ApplicationRepository(StorefrontDbContext dbContext) : IApplicationRepository
{
    public IQueryable<Application> GetLatestApplications(int maxLength = 10)
    {
        return dbContext.Applications
            .Include(a => a.Releases)
            .ThenInclude(r => r.Variants)
            .OrderByDescending(a => a.CreatedAt)
            .Take(maxLength);
    }

    public IQueryable<Application> GetMostPopularApplications(int maxLength = 10)
    {
        return dbContext.Applications
            .Include(a => a.Releases)
            .ThenInclude(r => r.Variants)
            .OrderByDescending(a => a.DownloadCount)
            .ThenByDescending(a => a.CreatedAt)
            .Take(maxLength);
    }

    public Application? GetApplication(Guid id)
    {
        return dbContext.Applications
            .Include(a => a.Releases)
            .ThenInclude(r => r.Variants)
            .FirstOrDefault(a => a.Id == id);
    }

    public Application CreateApplication(Application application)
    {
        dbContext.Applications.Add(application);
        dbContext.SaveChanges();
        return application;
    }

    public IQueryable<Application> GetApplicationsByDeveloper(string userId, int take = 10, int skip = 0)
    {
        return dbContext.Applications
            .Include(a => a.Releases)
            .ThenInclude(r => r.Variants)
            .Where(a => a.OwnerId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip(skip)
            .Take(take);
    }
}