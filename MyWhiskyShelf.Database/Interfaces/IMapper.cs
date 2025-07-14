namespace MyWhiskyShelf.Database.Interfaces;

public interface IMapper<TDomain, TEntity> 
{
    public TDomain MapToDomain(TEntity whiskyBottleEntity);
    public TEntity MapToEntity(TDomain domain);
}