using System;

namespace AutoCatalogLib.Persist.Generic
{
    interface IObservableSet<TEntity>
    {
        event EventHandler<SetChangedEventArgument<TEntity>> BeforeSetChanged;
        event EventHandler<SetChangedEventArgument<TEntity>> AfterSetChanged;
        void OnBeforeSetChanged(TEntity entity, EntityChangeState state);
        void OnAfterSetChanged(TEntity entity, EntityChangeState state);
    }
}
