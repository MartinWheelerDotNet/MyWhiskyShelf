using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results.Geo;

public enum GetCountryGeoOutcome { Success, Error }

public sealed record GetCountryGeoResult(
    GetCountryGeoOutcome Outcome,
    IReadOnlyList<Country>? Countries = null,
    string? Error = null);