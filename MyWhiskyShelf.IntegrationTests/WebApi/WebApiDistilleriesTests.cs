using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.IntegrationTests.TestData;
using MyWhiskyShelf.WebApi.Contracts.Common;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(WorkingFixture))]
public class WebApiDistilleriesTests(WorkingFixture fixture) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await fixture.ClearDatabaseAsync();
    }

    [Theory]
    [InlineData(10, 3)]
    [InlineData(5, 5)]
    [InlineData(30, 1)]
    [InlineData(2, 13)]
    public async Task When_GettingAllDistilleriesWithAmount_Expect_DistilleriesResponsesMatchInExpectedPages(
        int amountPerPage,
        int expectedPages)
    {
        const int amountOfDistilleries = 24;
        var expectedDistilleryResponses = await fixture.SeedDistilleriesAsync(amountOfDistilleries);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var page = 1;
        string? cursor = null;
        List<DistilleryResponse> distilleries = [];

        while (true)
        {
            var endpoint = cursor is null
                ? $"/distilleries?amount={amountPerPage}"
                : $"/distilleries?amount={amountPerPage}&cursor={cursor}";

            var response = await httpClient.GetAsync(endpoint);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var pagedResponse = await response.Content.ReadFromJsonAsync<CursorPagedResponse<DistilleryResponse>>();
            distilleries.AddRange(pagedResponse!.Items);

            if (string.IsNullOrWhiteSpace(pagedResponse.NextCursor)) break;

            cursor = pagedResponse.NextCursor;
            page++;
        }

        Assert.Multiple(
            () => Assert.Equal(expectedPages, page),
            () => Assert.Equal(expectedDistilleryResponses, distilleries));
    }

    [Fact]
    public async Task When_GettingAllDistilleriesAndOnLastPage_Expect_NextCursorIsNull()
    {
        const int amountOfDistilleries = 10;
        await fixture.SeedDistilleriesAsync(amountOfDistilleries);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync("/distilleries?amount=20");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pagedResponse = await response.Content.ReadFromJsonAsync<CursorPagedResponse<DistilleryResponse>>();

        Assert.Multiple(
            () => Assert.Equal(10, pagedResponse!.Items.Count),
            () => Assert.Null(pagedResponse!.NextCursor));
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(201)]
    [InlineData(500)]
    public async Task When_GettingAllDistilleriesAndAmountIsOutOfRange_Expect_ValidationProblem(int amount)
    {
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Status = 400,
            Title = "Paging parameters are out of range",
            Type = "urn:mywhiskyshelf:validation-errors:paging",
            Errors = new Dictionary<string, string[]>
            {
                { "amount", ["amount must be between 1 and 200"] }
            }
        };

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync($"/distilleries?amount={amount}");
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equivalent(expectedValidationProblemDetails, validationProblem);
    }

    [Fact]
    public async Task When_GettingAllDistilleriesAndInvalidCursor_Expect_ValidationProblem()
    {
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Status = 400,
            Title = "Paging parameters are out of range",
            Type = "urn:mywhiskyshelf:validation-errors:paging",
            Errors = new Dictionary<string, string[]>
            {
                { "cursor", ["The provided 'cursor' is invalid or malformed."] }
            }
        };

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync("/distilleries?amount=10&cursor=not-base64!!");
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equivalent(expectedValidationProblemDetails, validationProblem);
    }

    [Fact]
    public async Task When_GettingDistilleryByIdAndDistilleryExists_Expect_CorrectDistilleryReturned()
    {
        var distilleryDetails = await fixture.SeedDistilleriesAsync(1);
        var expectedResponse = distilleryDetails.Single();
        var id = expectedResponse.Id;

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/{id}");
        var distilleryResponse = await response.Content.ReadFromJsonAsync<DistilleryResponse>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Equal(expectedResponse, distilleryResponse!));
    }

    [Fact]
    public async Task When_GettingDistilleryByIdAndDistilleryDoesNotExist_Expect_NotFoundResponse()
    {
        var endpoint = $"/distilleries/{Guid.NewGuid()}";
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task When_SearchingByNameAndNotFound_Expect_EmptyDistilleryResponse()
    {
        await fixture.SeedDistilleriesAsync([DistilleryEntityTestData.Generic("Any Distillery")]);

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync("/distilleries/search?pattern=Else");
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        Assert.Empty(distilleries!);
    }

    [Theory]
    [InlineData("CASE TEST")]
    [InlineData("CASE test")]
    [InlineData("CaSe TeSt")]
    [InlineData("case test")]
    public async Task When_SearchingByNameInsensitiveCase_Expect_DistilleryResponseWithJustThatDistillery(
        string pattern)
    {
        const string expectedName = "Case Test";
        var seededDistilleryResponses =
            await fixture.SeedDistilleriesAsync([DistilleryEntityTestData.Generic(expectedName)]);
        var expectedResponse = seededDistilleryResponses.Single();

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/search?pattern={pattern}");
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        Assert.Single(distilleries!, actual => expectedResponse == actual);
    }

    [Fact]
    public async Task When_SearchingByNameAndOneFound_Expect_DistilleryResponseWithJustThatDistillery()
    {
        const string expectedName = "One";
        var seededDistilleryResponses = await fixture.SeedDistilleriesAsync([
            DistilleryEntityTestData.Generic(expectedName),
            DistilleryEntityTestData.Generic("Another Distillery")
        ]);
        var expectedResponse = seededDistilleryResponses.First(sd => sd.Name == expectedName);

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/search?pattern={expectedName}");
        var distilleryResponses = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        Assert.Single(distilleryResponses!, actual => expectedResponse == actual);
    }

    [Fact]
    public async Task When_SearchingByNameAndMultipleFound_Expect_DistilleryResponseWithThoseDistilleries()
    {
        const string searchPattern = "Two";
        var expectedDistilleryResponses = await fixture.SeedDistilleriesAsync([
            DistilleryEntityTestData.Generic(searchPattern),
            DistilleryEntityTestData.Generic($"Also {searchPattern}")
        ]);

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/search?pattern={searchPattern}");
        var distilleryResponses = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        Assert.Equal(expectedDistilleryResponses.OrderBy(r => r.Name).ThenBy(r => r.Id), distilleryResponses);
    }

    [Fact]
    public async Task When_AddingDistilleryAndDistilleryDoesNotExist_Expect_CreatedWithLocationHeaderSet()
    {
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/distilleries",
            DistilleryRequestTestData.GenericCreate with { Name = "Distillery D" });
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.SendAsync(request);

        Assert.Multiple(
            () => Assert.NotNull(response.Headers.Location),
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode));
    }

    [Fact]
    public async Task When_AddingDistilleryAndDistilleryAlreadyExists_Expect_Conflict()
    {
        var distilleries = await fixture.SeedDistilleriesAsync(1);
        var name = distilleries.Single().Name;

        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/distilleries",
            DistilleryRequestTestData.GenericCreate with { Name = name });
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task When_DeletingDistilleryAndDistilleryExists_Expect_NoContent()
    {
        var distilleries = await fixture.SeedDistilleriesAsync(1);
        var id = distilleries.Single().Id;

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/distilleries/{id}");

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task When_DeletingDistilleryAndDistilleryDoesNotExist_Expect_NotFound()
    {
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/distilleries/{Guid.NewGuid()}");

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task When_UpdatingDistilleryAndDistilleryExists_Expect_OkResponse()
    {
        var distilleries = await fixture.SeedDistilleriesAsync(1);
        var (id, name) = (distilleries.Single().Id, distilleries.Single().Name);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/distilleries/{id}",
            DistilleryRequestTestData.GenericUpdate with { Name = name });

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task When_UpdatingDistilleryAndDistilleryDoesNotExist_Expect_NotFoundResponse()
    {
        var distilleryId = Guid.NewGuid();
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/distilleries/{distilleryId}",
            DistilleryRequestTestData.GenericUpdate with { Name = "Update Distillery" });

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}