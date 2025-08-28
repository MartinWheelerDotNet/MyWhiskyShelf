using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results;

public enum UpdateDistilleryOutcome { Updated, NotFound, NameConflict, Error }

public sealed record UpdateDistilleryResult(
    UpdateDistilleryOutcome? Outcome,
    Distillery? Distillery = null,
    string? Error = null);