namespace AutoCatalogLib.Persist.Generic
{
    public class SetChangedEventArgument<TEntity>
    {
        public EntityChangeState State { get; set; }
        public TEntity Entity { get; set; }
    }
}