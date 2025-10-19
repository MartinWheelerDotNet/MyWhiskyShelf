using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.IntegrationTests.TestData;
using MyWhiskyShelf.WebApi.Contracts.GeoResponse;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(WorkingFixture))]
public class WebApiGeoTests(WorkingFixture fixture) : IAsyncLifetime
{
    [Fact]
    public async Task When_GetAllGeoDataAndDataFound_ExpectedCorrectGeoDataReturned()
    {
        var geoData = await fixture.SeedGeoDataAsync();

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync("/geo");
        var geoResponse = await response.Content.ReadFromJsonAsync<List<CountryResponse>>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Equivalent(geoData, geoResponse));
    }

    [Fact]
    public async Task When_GetAllGeoDataAndNoDataFound_ExpectedEmptyListReturned()
    {
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync("/geo");
        
        var geoResponse = await response.Content.ReadFromJsonAsync<List<CountryResponse>>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Empty(geoResponse!));
    }

    [Fact]
    public async Task When_CreateCountryAndCountryDoesNotExist_ExpectCreatedWithCountryReturned()
    {
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/geo/countries",
            CountryRequestTestData.GenericCreate);
        var response = await httpClient.SendAsync(request);
        
        var countryResponse = await response.Content.ReadFromJsonAsync<CountryResponse>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode),
            () => Assert.Equivalent(CountryResponseTestData.GenericCreate(countryResponse!.Id), countryResponse));
    }

    [Fact]
    public async Task When_CreateCountryAndCountryExists_ExpectConflict()
    {
        await fixture.SeedCountriesAsync([CountryEntityTestData.Generic("Conflict", "conflict")]);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/geo/countries",
            CountryRequestTestData.GenericCreate with { Name = "Conflict" });
        
        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task When_CreateRegionAndCountryExistsAndRegionCreated_ExpectOkWithRegion()
    {
        var countries = await fixture.SeedCountriesAsync([CountryEntityTestData.Generic("Exists", "exists")]);
        var country = countries.Single();
        
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/geo/regions",
            RegionRequestTestData.GenericCreate with { CountryId = country.Id });
        
        var response = await httpClient.SendAsync(request);
        var regionResponse = await response.Content.ReadFromJsonAsync<RegionResponse>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode),
            () => Assert.Equivalent(RegionResponseTestData.Generic(regionResponse!.Id, country.Id), regionResponse));
    }
    
    [Fact]
    public async Task When_CreateRegionAndRegionAlreadyExistsInCountry_ExpectConflict()
    {
        var countries = await fixture.SeedCountriesAsync([CountryEntityTestData.Generic("Country", "country")]);
        var country = countries.Single();
        await fixture.SeedRegionsAsync([RegionEntityTestData.Generic("Region", "region", country.Id)]);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/geo/regions",
            RegionRequestTestData.GenericCreate with { CountryId = country.Id, Name = "Region" });
        
        var response = await httpClient.SendAsync(request);
        
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
    
    [Fact]
    public async Task When_CreateRegionAndCountryDoesNotExists_ExpectValidationProblem()
    {
        var countryId = Guid.NewGuid();
        var expectedValidationProblem = new ValidationProblemDetails
        {
            Status = 400,
            Title = "Provided country not found by Id",
            Type = "urn:mywhiskyshelf:validation-errors:country-not-found",
            Errors = new Dictionary<string, string[]>
            {
                { "countryId", [$"The provided countryId {countryId} does not exist in the database."] }
            }
        };
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/geo/regions",
            RegionRequestTestData.GenericCreate with { CountryId = countryId });
        
        var response = await httpClient.SendAsync(request);
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode),
            () => Assert.Equivalent(expectedValidationProblem, validationProblem));
    }

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await fixture.ClearDatabaseAsync();
    }
}