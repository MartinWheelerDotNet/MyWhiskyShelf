namespace MyWhiskyShelf.Core.Models;

public sealed record DistilleryFilterOptions(
    Guid? CountryId = null,
    Guid? RegionId = null,
    string? NameSearchPattern = null,
    int Amount = 10,
    string? AfterName = null);