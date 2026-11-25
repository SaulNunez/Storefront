using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Storefront.Models.Enums;

#nullable disable

namespace Storefront.Migrations
{
    /// <inheritdoc />
    public partial class AddMacCpuArchitectures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // IF NOT EXISTS so Up can run again after Down, which can't remove enum labels.
            migrationBuilder.Sql("ALTER TYPE cpu_architecture ADD VALUE IF NOT EXISTS 'powerpc';");
            migrationBuilder.Sql("ALTER TYPE cpu_architecture ADD VALUE IF NOT EXISTS 'm68k';");

            // Keep existing rows: each single CPU becomes a one-element array.
            migrationBuilder.RenameColumn(
                name: "CpuPlatform",
                table: "Variants",
                newName: "CpuArchitectures");

            migrationBuilder.Sql(
                """ALTER TABLE "Variants" ALTER COLUMN "CpuArchitectures" TYPE cpu_architecture[] USING ARRAY["CpuArchitectures"];""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Fat binaries keep only their first CPU.
            migrationBuilder.Sql(
                """ALTER TABLE "Variants" ALTER COLUMN "CpuArchitectures" TYPE cpu_architecture USING "CpuArchitectures"[1];""");

            migrationBuilder.RenameColumn(
                name: "CpuArchitectures",
                table: "Variants",
                newName: "CpuPlatform");

            // PostgreSQL can't drop enum labels, so powerpc and m68k stay in cpu_architecture (unused).
        }
    }
}
