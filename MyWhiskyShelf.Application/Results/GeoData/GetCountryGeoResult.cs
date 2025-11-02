using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results.GeoData;

public enum GetCountryGeoOutcome
{
    Success,
    Error
}

public sealed record GetCountryGeoResult(
    GetCountryGeoOutcome Outcome,
    IReadOnlyList<Country>? Countries = null,
    string? Error = null);