using Storefront.Models.Enums;

namespace Storefront.Models.DAO.Windows;

public record WindowsVariantDao
(
    Guid Id,
    string ContentLocation,
    WindowsCpuPlatform CpuPlatform
);