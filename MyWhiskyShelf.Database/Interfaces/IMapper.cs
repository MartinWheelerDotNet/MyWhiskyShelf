namespace MyWhiskyShelf.Database.Interfaces;

public interface IMapper<in TInput, out TOutput>
{
    public TOutput Map(TInput input);
}