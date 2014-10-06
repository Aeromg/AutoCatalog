
namespace AutoCatalogLib.Persist.Generic
{
    public abstract class Bridge<TModel, TEntity>
    {
        private readonly TEntity _entity;

        private Bridge() { }

        protected Bridge(TEntity entity)
        {
            _entity = entity;
        }

        public TModel GetModel()
        {
            return GetModel(_entity);
        }
        public TEntity FromModel(TModel model)
        {
            UpdateEntity(_entity, model);
            return _entity;
        }

        protected abstract TModel GetModel(TEntity entity);
        protected abstract void UpdateEntity(TEntity entity, TModel model);
    }
}
