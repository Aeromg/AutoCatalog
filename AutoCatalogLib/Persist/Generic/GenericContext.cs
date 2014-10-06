using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using AutoCatalogLib.Utils;

namespace AutoCatalogLib.Persist.Generic
{
    public abstract class GenericContext : IDisposable, IObservableSet<Entity>
    {
        private static volatile object _saveChangesLockObject = new object();

        private static readonly IList<GenericContext> Contextes = new List<GenericContext>();

        private readonly GenericDbContext _context;

        public bool IsMailformed
        {
            get { return _context.IsMailformed; }
        }

        private readonly IDictionary<Type, object> _repositoriesLookup = new Dictionary<Type, object>();

        protected GenericContext(string contextName)
        {
            _context = new GenericDbContext(contextName);

            _context.BeforeSetChanged += (sender, args) => OnBeforeSetChanged(args.Entity, args.State);
            _context.AfterSetChanged += (sender, args) => OnAfterSetChanged(args.Entity, args.State);

            FillContextRepositoriesAndLookup();

            lock (Contextes)
                Contextes.Add(this);
        }

        protected GenericContext() : this("")
        {
        }

        public void MailformContext()
        {
            _context.Mailform();
        }

        public bool IsOutdated { get; private set; }

        public void SaveChanges()
        {
            lock (_saveChangesLockObject)
            {
                _context.SaveChanges();
                lock (Contextes)
                    foreach (var context in Contextes.Where(context => context != this))
                        context.IsOutdated = true;
            }
        }

        public bool TrySaveChanges()
        {
            try
            {
                SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
        }

        public GenericRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity
        {
            return (GenericRepository<TEntity>) _repositoriesLookup[typeof (TEntity)];
        }

        public GenericRepository GetRepository(Type entityType)
        {
            return (GenericRepository) _repositoriesLookup[entityType];
        }

        protected GenericRepository<TEntity> BuildRepository<TEntity>() where TEntity : Entity
        {
            return new GenericRepository<TEntity>(_context.Set<TEntity>());
        }

        protected object BuildRepository(Type entityType)
        {
            var dbset = _context.Set(entityType);
            var type = typeof (GenericRepository<>).MakeGenericType(new[] {entityType});
            var ctr = type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof (DbSet)}, null);

            return ctr.Invoke(new object[] {dbset});
        }

        private void FillContextRepositoriesAndLookup()
        {
            var props = GetType().GetProperties()
                .Where(
                    p =>
                        p.PropertyType.IsGenericType &&
                        typeof (GenericRepository<>).IsAssignableFrom(p.PropertyType.GetGenericTypeDefinition()) &&
                        typeof (Entity).IsAssignableFrom(p.PropertyType.GenericTypeArguments.First()));

            foreach (var propertyInfo in props)
            {
                var entityType = propertyInfo.PropertyType.GenericTypeArguments.First();
                var repository = BuildRepository(entityType);
                propertyInfo.SetValue(this, repository);
                _repositoriesLookup[entityType] = repository;
            }
        }

        protected static void Drop()
        {
            lock (_saveChangesLockObject)
            {
                GenericDbContext.Truncate();
            }
        }

        protected static void Vacuum()
        {
            lock (_saveChangesLockObject)
            {
                GenericDbContext.Vacuum();
            }
        }

        public static void Sql(string sql)
        {
            lock (_saveChangesLockObject)
            {
                GenericDbContext.Sql(sql);
            }
        }

        #region IDisposable members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GenericContext()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    lock (Contextes)
                        Contextes.Remove(this);

                    _context.Database.Connection.Close();
                    _context.Database.Connection.Dispose();
                    _context.Dispose();
                }
            }
        }

        #endregion

        #region IObservableSet members

        public event EventHandler<SetChangedEventArgument<Entity>> AfterSetChanged;

        public void OnAfterSetChanged(Entity entity, EntityChangeState state)
        {
            var handler = AfterSetChanged;
            if (handler != null)
                handler(this, new SetChangedEventArgument<Entity>()
                {
                    Entity = entity,
                    State = state
                });
        }

        public event EventHandler<SetChangedEventArgument<Entity>> BeforeSetChanged;

        public void OnBeforeSetChanged(Entity entity, EntityChangeState state)
        {
            var handler = BeforeSetChanged;
            if (handler != null)
                handler(this, new SetChangedEventArgument<Entity>()
                {
                    Entity = entity,
                    State = state
                });
        }

        #endregion

        protected static void CloseAll()
        {
            lock (Contextes)
                foreach (var context in Contextes)
                {
                    context.Dispose(true);
                }
        }
    }
}