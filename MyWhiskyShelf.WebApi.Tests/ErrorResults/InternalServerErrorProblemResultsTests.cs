using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.WebApi.ErrorResults;

namespace MyWhiskyShelf.WebApi.Tests.ErrorResults;

public class InternalServerErrorProblemResultsTests
{
    [Fact]
    public void When_InternalServerError_Expect_ProblemWithValuesSet()
    {
        var expectedProblem = Results.Problem(new ProblemDetails
        {
            Type = "urn:mywhiskyshelf:errors:name-action-failed",
            Title = "Failed to action name",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "An unexpected error occurred. (TraceId: traceId)",
            Instance = "path"
        });

        var problem = ProblemResults.InternalServerError("name", "action", "traceId", "path");

        Assert.Equivalent(expectedProblem, problem);
    }
}