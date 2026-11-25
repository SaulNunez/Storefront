using NpgsqlTypes;

namespace Storefront.Models.Enums;

/// <summary>
/// Mapped as a native PostgreSQL enum: append new members at the end and add a migration.
/// </summary>
public enum CpuArchitecture
{
    /// <summary>Architecture-independent (scripts, JVM or .NET AnyCPU builds). Runs on any CPU.</summary>
    Universal,
    X86,
    X64,
    Arm64,
    Arm32,
    [PgName("powerpc")]
    PowerPC,
    M68k
}
