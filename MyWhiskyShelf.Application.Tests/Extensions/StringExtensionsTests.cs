using MyWhiskyShelf.Application.Extensions;

namespace MyWhiskyShelf.Application.Tests.Extensions;

public class StringExtensionsTests
{
    private const string ExampleString = "This is a string";
    
    [Theory]
    [InlineData("\rThis is a string")]
    [InlineData("This is a \nstring")]
    [InlineData("\nThis is a string\r")]
    [InlineData("This is a string")]
    public void When_SanitizeForLog_Expect_StringWithNewlinesRemoved(string value)
    {
        var result = value.SanitizeForLog();
        
        Assert.Equal(ExampleString, result);
    }
}