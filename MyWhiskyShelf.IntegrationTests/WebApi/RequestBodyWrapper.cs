namespace MyWhiskyShelf.IntegrationTests.WebApi;

using System;
using System.Text.Json;
using Xunit.Abstractions;

public sealed class RequestBodyWrapper : IXunitSerializable
{
    public object? Value { get; private set; }

    public RequestBodyWrapper() { }
    public RequestBodyWrapper(object? value) => Value = value;

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

        if (typeName is null) { Value = null; return; }

        var type = Type.GetType(typeName, throwOnError: true)!;
        Value = JsonSerializer.Deserialize(json!, type);
    }

    public override string ToString() => Value?.ToString() ?? "<null>";
}
