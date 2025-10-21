using MyWhiskyShelf.Application.Extensions;

namespace MyWhiskyShelf.Application.Tests.Extensions;

public class SanitizeExtensionsTests
{
    private const string ExampleString = "This is a string";

    [Theory]
    [InlineData("\rThis is a string")]
    [InlineData("This is a \nstring")]
    [InlineData("\nThis is a string\r")]
    [InlineData("This is a string")]
    public void When_SanitizeForLogWithString_Expect_StringWithNewlinesRemoved(string value)
    {
        var result = value.SanitizeForLog();

        Assert.Equal(ExampleString, result);
    }
    
    [Theory]
    [InlineData("\r7085fbfa-c2b4-4390-a492-1311fe5dc758")]
    [InlineData("   7085fbfa-c2b4-4390-a492-1311fe5dc758   \t\r\n")]
    [InlineData("\n7085fbfa-c2b4-4390-a492-1311fe5dc758\r")]
    [InlineData("7085fbfa-c2b4-4390-a492-1311fe5dc758")]
    public void When_SanitizeForLogWithGuid_Expect_StringWithNewlinesRemoved(string guidString)
    {
        const string expectedGuidString = "7085fbfa-c2b4-4390-a492-1311fe5dc758";
        var result = Guid.Parse(guidString).SanitizeForLog();

        Assert.Equal(expectedGuidString, result);
    }
}