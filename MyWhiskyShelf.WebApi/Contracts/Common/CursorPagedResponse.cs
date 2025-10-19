namespace MyWhiskyShelf.WebApi.Contracts.Common;

public sealed record CursorPagedResponse<T>(
    IReadOnlyList<T> Items,
    string? NextCursor,
    int Amount);