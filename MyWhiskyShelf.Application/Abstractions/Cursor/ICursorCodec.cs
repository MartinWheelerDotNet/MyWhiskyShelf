namespace MyWhiskyShelf.Application.Abstractions.Cursor;

public interface ICursorCodec
{
    string Encode<T>(T payload);
    bool TryDecode<T>(string cursor, out T? payload) where T : class;
}