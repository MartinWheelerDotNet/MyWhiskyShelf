using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Xunit.Abstractions;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[ExcludeFromCodeCoverage]
public sealed class RequestBodyWrapper : IXunitSerializable
{
    // IXunitSerializable requires a parameterless constructor to implement, but this is use inherently
    // ReSharper disable once UnusedMember.Global
    public RequestBodyWrapper()
    {
    }

    public RequestBodyWrapper(object? value)
    {
        Value = value;
    }

    public object? Value { get; private set; }

    public void Serialize(IXunitSerializationInfo info)
    {
        if (Value is null)
        {
            info.AddValue("Type", null);
            info.AddValue("Json", null);
            return;
        }

        var type = Value.GetType();
        info.AddValue("Type", type.AssemblyQualifiedName);
        info.AddValue("Json", JsonSerializer.Serialize(Value, type));
    }

    public void Deserialize(IXunitSerializationInfo info)
    {
        var typeName = info.GetValue<string>("Type");
        var json = info.GetValue<string>("Json");

        if (typeName is null)
        {
            Value = null;
            return;
        }

        var type = Type.GetType(typeName, true)!;
        Value = JsonSerializer.Deserialize(json!, type);
    }

    public override string ToString()
    {
        return Value?.ToString() ?? "<null>";
    }
}