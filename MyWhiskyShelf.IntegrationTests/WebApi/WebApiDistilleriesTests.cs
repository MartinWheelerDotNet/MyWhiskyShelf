using System.Net.Http.Json;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.IntegrationTests.TestData;
using MyWhiskyShelf.WebApi.Contracts.Common;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(WorkingFixture))]
public class WebApiDistilleriesTests(WorkingFixture fixture) : IAsyncLifetime
{
    public async Task InitializeAsync() => await Task.CompletedTask;
    public async Task DisposeAsync()    => await fixture.ClearDatabaseAsync();

    [Theory]
    [InlineData(10, 3)]
    [InlineData(5, 5)]
    [InlineData(30, 1)]
    [InlineData(2, 13)]
    public async Task When_GetAllWithAmount_Expect_ResponsesMatchExpectedPages(int amountPerPage, int expectedPages)
    {
        const int amountOfDistilleries = 24;
        var expectedDistilleries = await fixture.SeedDistilleriesAsync(amountOfDistilleries);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var page = 1;
        string? cursor = null;
        List<DistilleryResponse> distilleryResponses = [];

        while (true)
        {
            var url = cursor is null
                ? $"/distilleries?amount={amountPerPage}"
                : $"/distilleries?amount={amountPerPage}&cursor={cursor}";

            var result = await httpClient.GetAsync(url);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var pagedResponse = await result.Content.ReadFromJsonAsync<CursorPagedResponse<DistilleryResponse>>();
            distilleryResponses.AddRange(pagedResponse!.Items);

            if (string.IsNullOrWhiteSpace(pagedResponse.NextCursor)) break;
            cursor = pagedResponse.NextCursor;
            page++;
        }

        Assert.Multiple(
            () => Assert.Equal(expectedPages, page),
            () => Assert.Equal(expectedDistilleries, distilleryResponses));
    }

    [Fact]
    public async Task When_GetAllOnLastPage_Expect_NextCursorIsNull()
    {
        await fixture.SeedDistilleriesAsync(10);
        
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.GetAsync("/distilleries?amount=20");
        var pagedResponse = await result.Content.ReadFromJsonAsync<CursorPagedResponse<DistilleryResponse>>();
        
        Assert.NotNull(pagedResponse);
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, result.StatusCode),
            () => Assert.Equal(10, pagedResponse.Items.Count),
            () => Assert.Null(pagedResponse.NextCursor));
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(201)]
    [InlineData(500)]
    public async Task When_GetAllAmountOutOfRange_Expect_ValidationProblem(int amount)
    {
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.GetAsync($"/distilleries?amount={amount}");

        await ValidationProblemAssertions.AssertValidationProblemAsync(
            result,
            expectedStatus: 400,
            expectedTitle:  "Paging parameters are out of range",
            expectedType:   "urn:mywhiskyshelf:validation-errors:paging",
            expectedErrors: new Dictionary<string, string[]>
            {
                { "amount", ["amount must be between 1 and 200"] }
            });
    }

    [Fact]
    public async Task When_GetAllInvalidCursor_Expect_ValidationProblem()
    {
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.GetAsync("/distilleries?amount=10&cursor=not-base64!!");

        await ValidationProblemAssertions.AssertValidationProblemAsync(
            result,
            expectedStatus: 400,
            expectedTitle:  "Paging parameters are out of range",
            expectedType:   "urn:mywhiskyshelf:validation-errors:paging",
            expectedErrors: new Dictionary<string, string[]>
            {
                { "cursor", ["The provided 'cursor' is invalid or malformed."] }
            });
    }
    
    [Fact]
    public async Task When_GetAllWithNameSearchNotFound_Expect_EmptyList()
    {
        await fixture.SeedDistilleriesAsync(3);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var result = await httpClient.GetAsync("/distilleries?amount=10&pattern=zzz");
        var pagedResponse = await result.Content.ReadFromJsonAsync<CursorPagedResponse<DistilleryResponse>>();

        Assert.Empty(pagedResponse!.Items);
    }

    [Fact]
    public async Task When_GetAllWithNameSearchCaseInsensitive_Expect_SingleMatch()
    {
        
        await fixture.SeedDistilleriesAsync(
        [
            DistilleryEntityTestData.Generic("Alpha", fixture.FirstSeededCountryId),
            DistilleryEntityTestData.Generic("Beta", fixture.FirstSeededCountryId)
        ]);

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.GetAsync("/distilleries?amount=10&pattern=alpha");
        var pagedResponse = await result.Content.ReadFromJsonAsync<CursorPagedResponse<DistilleryResponse>>();

        Assert.Single(pagedResponse!.Items, d => d.Name == "Alpha");
    }

    [Fact]
    public async Task When_GetAllWithNameSearchMultipleFound_Expect_SortedMatches()
    {
        
        await fixture.SeedDistilleriesAsync([
            DistilleryEntityTestData.Generic("Two A", fixture.FirstSeededCountryId),
            DistilleryEntityTestData.Generic("Two B", fixture.FirstSeededCountryId)
        ]);

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.GetAsync("/distilleries?amount=10&pattern=Two");
        var pagedResponse = await result.Content.ReadFromJsonAsync<CursorPagedResponse<DistilleryResponse>>();

        var expected = new[] { "Two A", "Two B" };
        
        Assert.Equal(expected, pagedResponse!.Items.Select(i => i.Name).ToArray());
    }

    [Fact]
    public async Task When_GetAllWithCountryFilter_Expect_OnlyThatCountriesDistilleries()
    {
        await fixture.SeedDistilleriesAsync(
        [
            DistilleryEntityTestData.Generic("C1 One", fixture.FirstSeededCountryId),
            DistilleryEntityTestData.Generic("C1 Two", fixture.FirstSeededCountryId),
            DistilleryEntityTestData.Generic("C2 One", fixture.SecondSeededCountryId)
        ]);

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.GetAsync($"/distilleries?amount=50&countryId={fixture.FirstSeededCountryId}");
        var pagedResponse = await result.Content.ReadFromJsonAsync<CursorPagedResponse<DistilleryResponse>>();

        Assert.All(pagedResponse!.Items, d => Assert.StartsWith("C1", d.Name));
    }

    [Fact]
    public async Task When_GetAllWithCountryAndRegionFilter_Expect_OnlyMatchingDistilleries()
    {
        await fixture.SeedDistilleriesAsync(
        [
            DistilleryEntityTestData.Generic("C1R1 One", fixture.FirstSeededCountryId, fixture.FirstRegionFirstCountryId),
            DistilleryEntityTestData.Generic("C1R1 Two", fixture.FirstSeededCountryId, fixture.FirstRegionFirstCountryId),
            DistilleryEntityTestData.Generic("C1R2", fixture.FirstSeededCountryId, fixture.SecondRegionFirstCountryId),
            DistilleryEntityTestData.Generic("C2", fixture.SecondSeededCountryId)
        ]);

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.GetAsync($"/distilleries?amount=50&countryId={fixture.FirstSeededCountryId}&regionId={fixture.FirstRegionFirstCountryId}");
        var pagedResponse = await result.Content.ReadFromJsonAsync<CursorPagedResponse<DistilleryResponse>>();

        Assert.All(pagedResponse!.Items, d => Assert.StartsWith("C1R1", d.Name));
    }

    [Fact]
    public async Task When_GetAllFreshSearchWithFilters_Expect_FirstPageSortedAndCursorReturned()
    {
        
        await fixture.SeedDistilleriesAsync(
        [
            DistilleryEntityTestData.Generic("A", fixture.FirstSeededCountryId),
            DistilleryEntityTestData.Generic("B", fixture.FirstSeededCountryId),
            DistilleryEntityTestData.Generic("C", fixture.FirstSeededCountryId)
        ]);
        var expected = new[] { "A", "B" };

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.GetAsync($"/distilleries?amount=2&countryId={fixture.FirstSeededCountryId}");
        var firstPage = await result.Content.ReadFromJsonAsync<CursorPagedResponse<DistilleryResponse>>();

        Assert.NotNull(firstPage!.NextCursor);
        Assert.Equal(expected, firstPage.Items.Select(x => x.Name).ToArray());
    }
    
    [Fact]
    public async Task When_GetByIdEntityExists_Expect_OkWithEntity()
    {
        var seeded = await fixture.SeedDistilleriesAsync(1);
        var expected = seeded.Single();

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.GetAsync($"/distilleries/{expected.Id}");
        var distilleryResponse = await result.Content.ReadFromJsonAsync<DistilleryResponse>();
        
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(expected, distilleryResponse);
    }

    [Fact]
    public async Task When_GetByIdEntityMissing_Expect_NotFound()
    {
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        
        var result = await httpClient.GetAsync($"/distilleries/{Guid.NewGuid()}");
        
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task When_CreateWithValidGeoData_Expect_CreatedAndLocationHeader()
    {
        await fixture.SetupCountriesForTests();
        
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post, "/distilleries",
            DistilleryRequestTestData.Create(fixture.FirstSeededCountryId,
                WorkingFixture.FirstSeededCountryName,
                name: "Distillery D"));
        
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.SendAsync(request);

        Assert.Multiple(
            () => Assert.NotNull(result.Headers.Location),
            () => Assert.Equal(HttpStatusCode.Created, result.StatusCode));
    }

    [Fact]
    public async Task When_CreateWithDuplicateName_Expect_Conflict()
    {
        var seeded = await fixture.SeedDistilleriesAsync(1);
        var duplicatedName = seeded.Single().Name;
        
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post, "/distilleries",
            DistilleryRequestTestData.Create(
                fixture.FirstSeededCountryId,
                WorkingFixture.FirstSeededCountryName,
                name: duplicatedName));
        
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.SendAsync(request);
        
        Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
    }

    [Fact]
    public async Task When_CreateWithUnknownCountry_Expect_ValidationProblemOnCountryId()
    {
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post, "/distilleries",
            DistilleryRequestTestData.Create(Guid.NewGuid(), "Unknown"));
        
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.SendAsync(request);

        await ValidationProblemAssertions.AssertValidationProblemAsync(
            result,
            expectedStatus: 400,
            expectedTitle:  "Paging parameters are out of range",
            expectedType:   "urn:mywhiskyshelf:validation-errors:paging",
            expectedErrors: new Dictionary<string, string[]>
            {
                { "countryId", ["Country does not exist."] }
            });
    }

    [Fact]
    public async Task When_CreateWithRegionNotInCountry_Expect_ValidationProblemOnRegionId()
    {
        await fixture.SeedRegionsAsync(
            [RegionEntityTestData.Generic("C2 Region", "c2-region", fixture.SecondSeededCountryId)]);
        
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post, "/distilleries",
            DistilleryRequestTestData.Create(
                fixture.FirstSeededCountryId,
                WorkingFixture.FirstSeededCountryName,
                fixture.SecondSeededCountryId));
        
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.SendAsync(request);

        await ValidationProblemAssertions.AssertValidationProblemAsync(
            result,
            expectedStatus: 400,
            expectedTitle:  "Paging parameters are out of range",
            expectedType:   "urn:mywhiskyshelf:validation-errors:paging",
            expectedErrors: new Dictionary<string, string[]>
            {
                { "regionId", ["Region does not exist in the specified country."] }
            });
    }

    [Fact]
    public async Task When_UpdateEntityExists_Expect_Ok()
    {
        var distilleries = await fixture.SeedDistilleriesAsync(1);
        
        var distillery = distilleries.Single();
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put, $"/distilleries/{distillery.Id}",
            DistilleryRequestTestData.Update(
                fixture.FirstSeededCountryId,
                WorkingFixture.FirstSeededCountryName,
                null,
                distillery.Name));
       
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.SendAsync(request);
        
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task When_UpdateEntityMissing_Expect_NotFound()
    {
        
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put, $"/distilleries/{Guid.NewGuid()}",
            DistilleryRequestTestData.Update(fixture.FirstSeededCountryId, WorkingFixture.FirstSeededCountryName));
        
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.SendAsync(request);
        
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task When_UpdateWithUnknownCountry_Expect_ValidationProblemOnCountryId()
    {
        var distilleries = await fixture.SeedDistilleriesAsync(1);
        var distillery = distilleries.Single();

        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put, $"/distilleries/{distillery.Id}",
            DistilleryRequestTestData.Update(Guid.NewGuid(), "Unknown", name: distillery.Name));

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.SendAsync(request);

        await ValidationProblemAssertions.AssertValidationProblemAsync(
            result,
            expectedStatus: 400,
            expectedTitle:  "Paging parameters are out of range",
            expectedType:   "urn:mywhiskyshelf:validation-errors:paging",
            expectedErrors: new Dictionary<string, string[]>
            {
                { "countryId", ["Country does not exist."] }
            });
    }

    [Fact]
    public async Task When_UpdateWithRegionNotInCountry_Expect_ValidationProblemOnRegionId()
    {
        var distilleries = await fixture.SeedDistilleriesAsync(1);
        var distillery = distilleries.Single();
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put, $"/distilleries/{distillery.Id}",
            DistilleryRequestTestData.Update(
                fixture.FirstSeededCountryId,
                WorkingFixture.FirstSeededCountryName,
                fixture.FirstRegionSecondCountryId,
                distillery.Name));

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.SendAsync(request);

        await ValidationProblemAssertions.AssertValidationProblemAsync(
            result,
            expectedStatus: 400,
            expectedTitle:  "Paging parameters are out of range",
            expectedType:   "urn:mywhiskyshelf:validation-errors:paging",
            expectedErrors: new Dictionary<string, string[]>
            {
                { "regionId", ["Region does not exist in the specified country."] }
            });
    }

    [Fact]
    public async Task When_DeleteEntityExists_Expect_NoContent()
    {
        var distilleries = await fixture.SeedDistilleriesAsync(1);
        var distilleryId = distilleries.Single().Id;
        var req = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/distilleries/{distilleryId}");
        
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.SendAsync(req);

        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task When_DeleteEntityMissing_Expect_NotFound()
    {
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/distilleries/{Guid.NewGuid()}");
    
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var result = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}
