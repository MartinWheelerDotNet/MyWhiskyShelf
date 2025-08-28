namespace MyWhiskyShelf.Application.Results;

public enum DeleteWhiskyBottleOutcome { Deleted, NotFound, Error }

public sealed record DeleteWhiskyBottleResult(DeleteWhiskyBottleOutcome Outcome, string? Error = null);