namespace MyWhiskyShelf.Application.Cursors;

public sealed record DistilleryQueryCursor(
    string? AfterName,
    string? NameSearchPattern,
    Guid? CountryId,
    Guid? RegionId);