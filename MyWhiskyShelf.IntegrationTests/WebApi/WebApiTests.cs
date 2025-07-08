using System.Net.Http.Json;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Resources.TestData;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

public static class WebApiTests
{
    private const string WebApiResourceName = "WebApi";
    
    [Collection("AspireTests")]
    public class WebApiSeededDataTests(MyWhiskyShelfBaseFixtureSeededDb fixture)
        : IClassFixture<MyWhiskyShelfBaseFixtureSeededDb>
    {
        [Fact]
        public async Task When_RequestingAllDistilleryNames_Expect_AllTestDistilleriesNamesToBeReturned()
        {
            const string endpoint = "/distilleries/names";
            List<string> expectedDistilleryNames = [
                DistilleryTestData.Aberargie.DistilleryName, 
                DistilleryTestData.Aberfeldy.DistilleryName,
                DistilleryTestData.Aberlour.DistilleryName
            ];
        
            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleries = await response.Content.ReadFromJsonAsync<List<string>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Equal(expectedDistilleryNames, distilleries));
        }

        [Fact]
        public async Task When_RequestingAllDistilleries_Expect_AllTestDistilleriesReturned()
        {
            const string endpoint = "/distilleries";
            List<Distillery> expectedDistilleryNames = [
                DistilleryTestData.Aberargie, 
                DistilleryTestData.Aberfeldy,
                DistilleryTestData.Aberlour
            ];
        
            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleries = await response.Content.ReadFromJsonAsync<List<Distillery>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Equal(expectedDistilleryNames, distilleries));
        }

        [Fact]
        public async Task When_RequestingDistilleryByName_Expect_CorrectDistilleryReturned()
        {
            var endpoint = $"/distilleries/{DistilleryTestData.Aberfeldy.DistilleryName}";
            
            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distillery = await response.Content.ReadFromJsonAsync<Distillery>();
            
            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Equal(DistilleryTestData.Aberfeldy, distillery));
        }
    }
    
    [Collection("AspireTests")]
    public class WebApiNotSeededDataTests(MyWhiskyShelfBaseFixtureEmptyDb fixture)
        : IClassFixture<MyWhiskyShelfBaseFixtureEmptyDb>
    {
        [Fact]
        public async Task When_RequestingAllDistilleryNames_Expect_EmptyListIsReturned()
        {
            const string endpointName = "/distilleries/names";
            
            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpointName);
            var distilleries = await response.Content.ReadFromJsonAsync<List<string>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Equal([], distilleries));
        }  
        
        [Fact]
        public async Task When_RequestingAllDistilleries_Expect_EmptyListIsReturned()
        {
            const string endpoint = "/distilleries";
            
            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleries = await response.Content.ReadFromJsonAsync<List<Distillery>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Equal([], distilleries));
        }

        [Fact]
        public async Task When_RequestingDistilleryByName_Expect_NotFoundResponse()
        {
            var endpoint = $"/distilleries/{DistilleryTestData.Aberfeldy.DistilleryName}";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}