using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Storefront.Models;
using Storefront.Repositories;
using Storefront.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<StorefrontDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<StorefrontDbContext>();

// Object storage client, configured from the "S3" section (KeyId, SecretKey, Region).
// Resolved lazily so the app can start without S3 credentials.
builder.Services.AddSingleton(_ =>
{
    var s3 = builder.Configuration.GetSection("S3");
    return new AmazonS3Client(s3["KeyId"]!, s3["SecretKey"]!, Enum.Parse<AmazonS3Region>(s3["Region"]!));
});

// Register Repositories and Services
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IReleaseRepository, ReleaseRepository>();
builder.Services.AddScoped<IAppCategoryRepository, AppCategoryRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IApplicationObjectStorageRepository, ApplicationObjectStorageRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IReleaseService, ReleaseService>();
builder.Services.AddScoped<IAppCategoryService, AppCategoryService>();
builder.Services.AddScoped<ICommentService, CommentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.MapRazorPages();

app.Run();
