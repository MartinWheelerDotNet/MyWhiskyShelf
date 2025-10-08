using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.IntegrationTests.TestData;
using MyWhiskyShelf.WebApi.Contracts.Common;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(WorkingFixture))]
public class WebApiDistilleriesTests(WorkingFixture fixture)
{
    [Theory]
    [InlineData(10, 4)]
    [InlineData(5, 6)]
    [InlineData(30, 2)]
    [InlineData(2, 13)]
    public async Task When_GettingAllDistilleriesWithPageAndAmount_Expect_DistilleriesResponsesMatchInExpectedPages(
       int amountPerPage,
       int expectedPagesIncludingEndOfListPage)
    {
        const int amountOfDistilleries = 24;
        var expectedDistilleryResponses = await fixture.SeedDistilleriesAsync(amountOfDistilleries);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var page = 1;
        List<DistilleryResponse> distilleries = [];
        while (true)
        {
            var response = await httpClient.GetAsync($"/distilleries?page={page}&amount={amountPerPage}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<DistilleryResponse>>();
            distilleries.AddRange(pagedResponse!.Items);

            if (pagedResponse.Items.Count is 0) break;

            page++;
        }

        await fixture.ClearDistilleriesAsync();
        
        Assert.Multiple(
            () => Assert.Equal(expectedPagesIncludingEndOfListPage, page),
            () => Assert.Equal(expectedDistilleryResponses, distilleries));
    }

    [Fact]
    public async Task When_GettingAllDistilleriesAndRequestBeyondAvailable_Expect_EmptyDistilleriesList()
    {
        const int amountOfDistilleries = 10;
        await fixture.SeedDistilleriesAsync(amountOfDistilleries);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync("/distilleries?page=2&amount=10");
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<DistilleryResponse>>();
        
        await fixture.ClearDistilleriesAsync();
        
        Assert.Empty(pagedResponse!.Items);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task When_GettingAllDistilleriesAndPageIsLessThanOne_Expect_ValidationProblem(int page)
    {
       var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Status = 400,
            Title = "Paging parameters are out of range",
            Type = "urn:mywhiskyshelf:validation-errors:paging",
            Errors = new Dictionary<string, string[]>
            {
                { "page", ["page must be greater than or equal to 1"] }
            }
        };

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync($"/distilleries?page={page}&amount=10");
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        Assert.Equivalent(expectedValidationProblemDetails, validationProblem);
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

        var response = await httpClient.GetAsync($"/distilleries?page=0&amount={amount}");
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        Assert.Equivalent(expectedValidationProblemDetails, validationProblem);
    }
    
    [Fact]
    public async Task When_GettingAllDistilleriesAndAmountAndPageAreOutOfRange_Expect_ValidationProblemWithBothErrors()
    {
        var expectedValidationProblemDetails = new ValidationProblemDetails
        {
            Status = 400,
            Title = "Paging parameters are out of range",
            Type = "urn:mywhiskyshelf:validation-errors:paging",
            Errors = new Dictionary<string, string[]>
            {
                { "page", ["page must be greater than or equal to 1"] },
                { "amount", ["amount must be between 1 and 200"] }
            }
        };

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync("/distilleries?page=0&amount=0");
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        Assert.Equivalent(expectedValidationProblemDetails, validationProblem);
    }
    
    [Fact]
    public async Task When_GettingDistilleryByIdAndDistilleryExists_Expect_CorrectDistilleryReturned()
    {
        var distilleryDetails = await fixture.SeedDistilleriesAsync(1);
        var (id, name) = (distilleryDetails.Single().Id, distilleryDetails.Single().Name);
        var expectedResponse = DistilleryResponseTestData.GenericResponse(id) with { Name = name };

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/{id}");
        var distilleryResponse = await response.Content.ReadFromJsonAsync<DistilleryResponse>();
        
        await fixture.ClearDistilleriesAsync();

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
        await fixture.SeedDistilleriesAsync(DistilleryRequestTestData.GenericCreate with
        {
            Name = "Any Distillery"
        });
        
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
        var seededDistilleries = await fixture.SeedDistilleriesAsync(
            DistilleryRequestTestData.GenericCreate with { Name = expectedName });
        seededDistilleries.TryGetValue(expectedName, out var expectedId);
        
        var expectedResponse = DistilleryResponseTestData.GenericResponse(expectedId) with { Name = expectedName };

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/search?pattern={pattern}");
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        await fixture.ClearDistilleriesAsync();

        Assert.Single(distilleries!, actual => expectedResponse == actual);
    }
    
    [Fact]
    public async Task When_SearchingByNameAndOneFound_Expect_DistilleryResponseWithJustThatDistillery()
    {
        const string expectedName = "One";
        var seededDistilleries = await fixture.SeedDistilleriesAsync(
            DistilleryRequestTestData.GenericCreate with { Name = expectedName },
            DistilleryRequestTestData.GenericCreate with { Name = "Another" });
        
        seededDistilleries.TryGetValue(expectedName, out var expectedId);
        var expectedResponse = DistilleryResponseTestData.GenericResponse(expectedId) with { Name = expectedName };

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/search?pattern={expectedName}");
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();
        
        await fixture.ClearDistilleriesAsync();

        Assert.Single(distilleries!, actual => expectedResponse == actual);
    }
    
    [Fact]
    public async Task When_SearchingByNameAndMultipleFound_Expect_DistilleryResponseWithThoseDistilleries()
    {
        const string searchPattern = "Two";
        var seededDistilleries = await fixture.SeedDistilleriesAsync(
            DistilleryRequestTestData.GenericCreate with { Name = "Two" },
            DistilleryRequestTestData.GenericCreate with { Name = "Also Two" });

        var expectedDistilleryNames = seededDistilleries
            .Select(dr => DistilleryResponseTestData.GenericResponse(dr.Value) with { Name = dr.Key });

        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();
        var response = await httpClient.GetAsync($"/distilleries/search?pattern={searchPattern}");
        var distilleryNames = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();
        
        await fixture.ClearDistilleriesAsync();

        Assert.Equal(expectedDistilleryNames, distilleryNames);
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

        await fixture.ClearDistilleriesAsync();

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

        await fixture.ClearDistilleriesAsync();
        
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
        
        await fixture.ClearDistilleriesAsync();

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

        await fixture.ClearDistilleriesAsync();
        
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