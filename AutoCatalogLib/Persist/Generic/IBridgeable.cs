
namespace AutoCatalogLib.Persist.Generic
{
    public interface IBridgeable<TModel, TEntity> where TEntity : Entity
    {
        Bridge<TModel, TEntity> Bridge { get; }
    }
}
