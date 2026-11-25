# Storefront

Serve you application or program the easy way.

Get the same than app store while distributing it yourself from your own server or infrastructure.

# Features
* Offer your users a know UX to download your app
* App variants for handling different compilation targets
    * CPU architectures (Mac, Windows, Android)
    * Screen densities (Android)
    * Language (Android)
* Comment section to get user feedback
* Obtain download metrics (future)
* Accept user donations (future)

# Running locally

## Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* PostgreSQL 14 or newer
* The EF Core CLI: `dotnet tool install --global dotnet-ef --version 10.0.12`
* Optional: an S3-compatible bucket, only needed for icon uploads

## 1. Start PostgreSQL

`appsettings.Development.json` expects a local database named `storefront`, owned by a
`storefront` user with password `storefront`. The quickest way is Docker:

```bash
docker run -d --name storefront-db -p 5432:5432 \
  -e POSTGRES_USER=storefront -e POSTGRES_PASSWORD=storefront -e POSTGRES_DB=storefront \
  postgres:16
```

Or, on an existing PostgreSQL install:

```sql
CREATE ROLE storefront LOGIN PASSWORD 'storefront' CREATEDB;
```

To point at a different database, override the connection string with user secrets
instead of editing the settings file:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Database=...;Username=...;Password=..."
```

## 2. Create the schema

```bash
dotnet ef database update
```

This applies the migrations in `Migrations/`, which create the Identity tables, the
application tables, the PostgreSQL enum types, and the 22 seeded app categories.

## 3. Configure object storage (optional)

The app starts without S3 credentials, but uploading an app icon fails until they are set.
Keep them out of source control with user secrets:

```bash
dotnet user-secrets set "S3:KeyId" "<access key id>"
dotnet user-secrets set "S3:SecretKey" "<secret key>"
dotnet user-secrets set "S3:Region" "UsEast1"   # an AmazonS3Region name
```

## 4. Run

```bash
dotnet run
```

Then open the URL printed in the console.

Submitting an application requires the `Administrator` or `Developer` role, and nothing
assigns roles yet. After registering an account, add the role by hand:

```sql
INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName")
VALUES (gen_random_uuid()::text, 'Developer', 'DEVELOPER');

INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT u."Id", r."Id" FROM "AspNetUsers" u, "AspNetRoles" r
WHERE u."NormalizedEmail" = upper('you@example.com') AND r."Name" = 'Developer';
```

Registration requires a confirmed email, and no email sender is configured. In
Development, the Identity UI shows a confirmation link on the page after you register.

## Changing the data model

After editing an entity or `StorefrontDbContext`, add a migration and apply it:

```bash
dotnet ef migrations add <DescriptiveName>
dotnet ef database update
```
