namespace MyWhiskyShelf.WebApi.Contracts.Common;

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int Amount
);