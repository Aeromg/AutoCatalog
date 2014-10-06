using System;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Persist.Generic;

namespace AutoCatalogLib.Persist.BusinessModels.Dynamic.Formatters
{
    public abstract class PersistValueFormatter<TEntity> : Entity, IBridgeable<IFormatter, TEntity>
        where TEntity : PersistValueFormatter<TEntity>
    {
        private Bridge<IFormatter, TEntity> _bridge;

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid Guid { get; set; }

        public GeneralizedType TargetValueType { get; set; }

        public virtual Bridge<IFormatter, TEntity> Bridge
        {
            get { return _bridge ?? (_bridge = BuildBridge()); }
        }

        protected abstract Bridge<IFormatter, TEntity> BuildBridge();
    }
}