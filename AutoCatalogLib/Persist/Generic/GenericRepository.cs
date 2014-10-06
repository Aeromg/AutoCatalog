using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace AutoCatalogLib.Persist.Generic
{
    public abstract class GenericRepository
    {
        protected GenericRepository() { }

        public abstract void AddOrAttach(Entity entity);

        public abstract void AddRange(IEnumerable<Entity> entities);

        public abstract void Remove(Entity entity);

        public abstract void Remove(IEnumerable<Entity> entities);
    }

    public class GenericRepository<TEntity> : 
        GenericRepository, IObservableSet<TEntity>, IQueryable<TEntity> where TEntity : Entity
    {
        private readonly DbSet<TEntity> _dbSet;
        private readonly string _sql;

        internal GenericRepository(DbSet<TEntity> dbSet)
        {
            _dbSet = dbSet;
            _sql = _dbSet.ToString();
        }

        internal GenericRepository(DbSet dbSet) : this(dbSet.Cast<TEntity>()) { }

        protected GenericRepository() { }

        #region CRUD members

        public void AddOrAttach(TEntity entity)
        {
            if (entity.Id == 0)
                _dbSet.Add(entity);
            else
                _dbSet.Attach(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void Remove(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        #endregion

        #region IQueryable<TEntity> members

        public IEnumerator<TEntity> GetEnumerator()
        {
            var enumerable = _dbSet.AsEnumerable();
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof (TEntity); }
        }

        public Expression Expression
        {
            get { return _dbSet.AsQueryable().Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _dbSet.AsQueryable().Provider; }
        }

        public IQueryable<TEntity> Sql(string sql)
        {
            return _dbSet.SqlQuery(sql).AsQueryable();
        }

        public IQueryable<TEntity> SqlAppend(string where)
        {
            return Sql(_sql + "\nWHERE " + where);
        }

        #endregion

        #region Non-generic members

        public override void AddOrAttach(Entity entity)
        {
            AddOrAttach((TEntity)entity);
        }

        public override void AddRange(IEnumerable<Entity> entities)
        {
            AddRange(entities.Cast<TEntity>());
        }

        public override void Remove(Entity entity)
        {
            Remove((TEntity)entity);
        }

        public override void Remove(IEnumerable<Entity> entities)
        {
            Remove(entities.Cast<TEntity>());
        }

        #endregion

        #region IObservableSet members

        public event EventHandler<SetChangedEventArgument<TEntity>> AfterSetChanged;

        public void OnAfterSetChanged(TEntity entity, EntityChangeState state)
        {
            var handler = AfterSetChanged;
            if (handler != null)
                handler(this, new SetChangedEventArgument<TEntity>()
                {
                    Entity = entity,
                    State = state
                });
        }

        public event EventHandler<SetChangedEventArgument<TEntity>> BeforeSetChanged;

        public void OnBeforeSetChanged(TEntity entity, EntityChangeState state)
        {
            var handler = BeforeSetChanged;
            if (handler != null)
                handler(this, new SetChangedEventArgument<TEntity>()
                {
                    Entity = entity,
                    State = state
                });
        }

        #endregion
    }
}