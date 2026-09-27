# CLAUDE.md

Guidance for AI assistants working in this repository.

## What this project is

**Storefront** is an ASP.NET Core 8 MVC web application for self-hosted app
distribution: developers publish an application, attach per-platform *releases*,
and each release carries one or more *variants* (per CPU architecture, Android
screen density, language, min OS version). End users browse a store-like UI and
download the variant that matches their machine.

Single project, single solution — `Storefront.csproj` / `Storefront.sln`.
Root namespace: `Storefront`. Target framework: `net8.0`, with
`Nullable` and `ImplicitUsings` enabled.

## Tech stack

| Concern | Choice |
| --- | --- |
| Web framework | ASP.NET Core 8 MVC (controllers + Razor views) |
| ORM | Entity Framework Core 9 (`Microsoft.EntityFrameworkCore` 9.0.11) |
| Database | PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL` |
| Auth | ASP.NET Core Identity (`AddDefaultIdentity<IdentityUser>`) with the scaffolded Identity UI under `Areas/Identity` |
| Object storage | `Genbox.SimpleS3.AmazonS3` (S3-compatible), for app icons/screenshots/binaries |
| Frontend | Razor + Bootstrap 5 + jQuery, vendored under `wwwroot/lib` |

Note the version skew: the app targets .NET 8 but references EF Core 9 packages
(and `Microsoft.EntityFrameworkCore.Tools` 10.0.0). Do not "fix" this by bumping
`TargetFramework` unless asked.

## Layout

```
Program.cs                 Composition root: DI registrations + HTTP pipeline
Controllers/               MVC + API controllers
Services/                  Business logic; each file declares its own I*Service interface
Repositories/              Data access; each file declares its own I*Repository interface
Models/                    EF entities (Application, Release, Variant, Comment, AppCategories)
Models/DAO/                Read/response records returned out of services to views & API
Models/Inputs/             Request/input records bound from HTTP
Models/Enums/              TargetPlatform, CpuArchitecture, and platform-specific enums
Models/Exceptions/         NotFoundException
Models/StorefrontDbContext.cs
Views/                     Razor views (Home, Application, Submit, Shared)
Areas/Identity/Pages/      Scaffolded Identity UI — do not hand-edit unless asked
wwwroot/                   Static assets; wwwroot/lib is vendored third-party, never edit
```

## Architecture and conventions

### Layering — Controller → Service → Repository → DbContext

This is the rule the recent history has been converging on, and new code must
follow it:

- **Repositories** own all `StorefrontDbContext` access. They return **entities**
  (`Application`, `Release`, `Variant`) — as `IQueryable<T>` for query methods
  that callers may still compose over, or as materialized entities for
  single-item and write methods.
- **Services** own business rules and are the only layer that converts entities
  to DAOs. They never touch `DbContext` directly.
- **Controllers** are thin: validate inputs, call one service, map exceptions to
  status codes/views, log failures.

### Interfaces live beside their implementation

Each service/repository file declares its interface and its class in the same
file, in the same namespace (`Storefront.Services` / `Storefront.Repositories`).
There is no separate `Interfaces/` folder — keep it that way.

Every service and repository is registered as scoped in `Program.cs`; register
any new one there too. Controllers inject `ILogger<T>`, never plain `ILogger`
(the container cannot resolve the non-generic one).

### Primary constructors for DI

Every service, repository, controller, and the `DbContext` uses C# primary
constructor injection:

```csharp
public class ReleaseService(IReleaseRepository releaseRepository) : IReleaseService
```

Match this style; do not introduce explicit constructors with field assignment.

### DAO records and `MapFromEntity`

Entity → DAO mapping is expressed as a **static `Expression<Func<TEntity, TDao>>`
named `MapFromEntity`**, plus a compiled `Func<>` named `FromEntity`:

```csharp
public static readonly Expression<Func<Variant, VariantDao>> MapFromEntity = variant => new VariantDao { ... };
public static readonly Func<Variant, VariantDao> FromEntity = MapFromEntity.Compile();
```

- Use `MapFromEntity` inside `.Select(...)` over an `IQueryable` so the projection
  translates to SQL.
- Use `FromEntity` on an already-materialized entity.
- There is no AutoMapper; add mappings by hand in this shape.

DAOs are `record` types with `required`/`init` members. Inputs
(`ApplicationInput`, `ReleaseInput`, `VariantInput`) live in `Models/Inputs` —
never bind an EF entity directly from a request body.

### Error handling

- Services throw `NotFoundException` (`Models/Exceptions`) when an entity is
  missing; repositories throw `ArgumentException` for a missing parent.
- Controllers catch `NotFoundException` → `NotFound()`, `ArgumentException` →
  `BadRequest(ex.Message)`, and a final `catch (Exception ex)` that logs via
  `logger.LogError(ex, "...")` and returns
  `StatusCode(500, "An error occurred while processing your request.")`.
  Reuse that exact shape for new endpoints.

### Routing — two kinds of controllers coexist

- **API controllers**: `[ApiController]` + explicit `[Route("api/...")]`,
  deriving from `ControllerBase`, returning `Ok(...)`/`Created...`.
  Examples: `ReleasesController` (`api/applications/{applicationId:guid}/releases`),
  `VariantsController` (`api/releases/{releaseId:guid}/variants`),
  `AppCategoriesController`, `DevelopersController`.
- **View controllers**: `HomeController`, `ApplicationController`,
  `SubmitController`, deriving from `Controller` and returning `View(...)`,
  served by the conventional route `{controller=Home}/{action=Index}/{id?}`.
  Exception: `ApplicationController.Details` is attribute-routed to the site
  root as `/{applicationId:guid}`. View controllers map errors to
  `View("Error", new ErrorViewModel { ... })` instead of `StatusCode(500, ...)`.
- The scaffolded Identity UI is served through `MapRazorPages()`.

Write-endpoints are gated with `[Authorize(Roles = "Administrator,Developer")]`;
the caller's id comes from `User.FindFirstValue(ClaimTypes.NameIdentifier)`.
Roles are enabled (`AddRoles<IdentityRole>()`).

### Domain model

- `Application` 1—* `Release` 1—* `Variant`; `Application` also has `Comments`
  and a seeded `AppCategories` category.
- User references are Identity `string` ids with an `IdentityUser` navigation:
  `Application.OwnerId`/`Owner` and `Comment.UserId`/`User`.
- `Application.DownloadCount` drives the "most popular" ordering.
- `TargetPlatform` (Windows/Android/MacOs/Linux) lives on `Release`;
  `CpuArchitecture` (default `Universal`) and the optional
  `AndroidScreenDensity`, `Language`, `MinOsVersion` live on `Variant`.
- `TargetPlatform`, `CpuArchitecture`, and `AndroidScreenDensity` are mapped as
  **native PostgreSQL enums** via `builder.HasPostgresEnum<T>()` in
  `StorefrontDbContext.OnModelCreating`. Adding or reordering members of those
  enums is a schema change and needs a migration.
- The 22 `AppCategories` rows are seeded with `HasData` — changing that list is
  also a migration.
- `Models/Enums/WindowsEnums.cs` and `MacOsEnums.cs` (`WindowsVersion`,
  `WindowsCpuPlatform`, `MacOSPlatforms`, `AndroidCpuPlatform`) are legacy from
  an earlier per-platform design and are largely unused by the current
  `Release`/`Variant` model — only `WindowsApplicationVariantDao` still
  references one. Prefer the shared `CpuArchitecture`/`TargetPlatform` enums for
  new work.
- Binaries are not stored in the database: `Variant.ObjectKeyInStorage` holds the
  S3 object key. Icons go to `{applicationId}/listing/icons/store_icon_512.png`
  in the `application` bucket.

## Development workflow

The .NET SDK is **not installed** in the default remote container — `dotnet` is
unavailable, so you cannot build, run, or add migrations here. Do not claim a
change compiles unless you actually built it; say plainly that you could not.

If the only installed runtime is newer than .NET 8 (e.g. a .NET 10-only SDK),
`dotnet build` works but `dotnet run` / `dotnet ef` fail to launch; set
`DOTNET_ROLL_FORWARD=Major` for local runs instead of changing
`TargetFramework`. For a throwaway schema without creating `Migrations/`, use
`dotnet ef dbcontext script` and apply the SQL by hand.

When an SDK is available, the standard commands are:

```bash
dotnet restore
dotnet build
dotnet run                                  # https://localhost:xxxx
dotnet ef migrations add <Name>             # requires a design-time DbContext + connection string
dotnet ef database update
```

There is **no test project** and no CI workflow in this repository. If you add
behavior worth testing, propose an xUnit project rather than assuming one exists.

### Configuration

- `appsettings.json` / `appsettings.Development.json` currently contain only
  logging config and `AllowedHosts`.
- `Program.cs` reads `ConnectionStrings:DefaultConnection` for Npgsql — it is
  **not present in either settings file**, so it must be supplied via user
  secrets or environment
  (`ConnectionStrings__DefaultConnection=Host=...;Database=...;Username=...;Password=...`).
- `AmazonS3Client` is built from the `S3` section (`S3:KeyId`, `S3:SecretKey`,
  `S3:Region`, where `Region` is an `AmazonS3Region` enum name such as
  `UsEast1`). It is resolved lazily, so the app starts without it, but icon
  upload fails until it is supplied via user secrets or environment
  (`S3__KeyId=...`).
- Never commit connection strings, S3 keys, or other secrets.

### Migrations

`Migrations/` does not exist yet — the schema has never been generated. The first
`dotnet ef migrations add` will create it. Do not hand-write migration files.

## Known gaps (do not mistake these for working code)

Useful context before changing something that looks broken — it probably is.
Fix them when they are in scope for your task; mention them otherwise.

1. **Comments are never displayed or created.** `ICommentService` is
   registered but no controller uses it; `ApplicationDao.Comments` is never
   populated, so the Details page's comment section is always empty. There is no
   endpoint for posting comments, and `Comment` has no timestamp for ordering.
2. **No role assignment.** Roles are enabled, but nothing seeds the
   `Administrator`/`Developer` roles or assigns them, so role-gated endpoints
   (including `/Submit`) are unreachable until rows are added to
   `AspNetRoles`/`AspNetUserRoles` by hand.
3. **`DownloadCount` is never incremented** — there is no download endpoint yet.
4. **Release/variant submission pages are mockups.** `SubmitController` serves
   `Views/Submit/{Android,Mac,Windows}Release` and `MacVariants`, but their forms
   post nowhere; binary upload to object storage is not implemented. Only
   `Submit/Index` (create application) is wired up.
5. **`DevelopersController` loose ends.** `SaveIcon` checks `icon.Length < 0`
   (never true), neither it nor `PublishApplication` checks the role or app
   ownership (plain `[Authorize]`), and `Application.StoreIconUrl` is never set
   after an icon upload.
6. **No category validation.** `CreateApplication` trusts
   `ApplicationInput.CategoryId`; an id outside the seeded range fails at the FK
   constraint and surfaces as a 500.
7. `AppCategoriesController` has no namespace declaration and derives from
   `Controller` rather than `ControllerBase`.

## Working agreements

- Keep changes minimal and in the established layering; do not introduce new
  architectural patterns (mediator, AutoMapper, separate interface assemblies)
  without being asked.
- Never edit `wwwroot/lib/**` (vendored Bootstrap/jQuery) or regenerate
  `Areas/Identity` scaffolding as a side effect.
- Commit messages in this repo are short imperative summaries
  ("Add current services and repositories for DI", "Code cleanup for ApplicationService").
  Match that.
- Development happens on feature branches; `main` is the default branch.
