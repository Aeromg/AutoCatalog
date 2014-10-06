using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SQLite;
using System.Linq;
using AutoCatalogLib.Persist.BusinessModels.Catalog;
using AutoCatalogLib.Utils;
using System.Data.SQLite.EF6;

namespace AutoCatalogLib.Persist.Generic
{
    class GenericDbContext : DbContext, IObservableSet<Entity>
    {
        private bool _mailformed;

        public bool IsMailformed
        {
            get { return _mailformed; }
        }

        public void Mailform()
        {
            _mailformed = true;

            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ValidateOnSaveEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public GenericDbContext(string contextName)
            : base(GetNewConnection(), true)
        {
            Configuration.ValidateOnSaveEnabled = false;
            (((IObjectContextAdapter) this).ObjectContext).ObjectMaterialized += 
                (sender, args) => OnAfterSetChanged((Entity)args.Entity, EntityChangeState.Loaded);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");

            var assm = AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName.Contains("AutoCatalogLib"));

            var entityTypes = assm.GetTypes().Where(t =>
                typeof(Entity) != t &&
                typeof(Entity).IsAssignableFrom(t) &&
                !t.IsGenericType).ToArray();
            
            foreach (var type in entityTypes)
                entityMethod.MakeGenericMethod(type).Invoke(modelBuilder, new object[] { });

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            return _mailformed ? QuickSaveChanges() : SaveChangesAndNotify();
        }

        private int SaveChangesAndNotify()
        {
            int result;

            var changes = GetChangeset(ChangeTracker.Entries<Entity>()).ToArray();

            foreach (var change in changes)
                OnBeforeSetChanged(change.Key, change.Value);

            try
            {
                result = base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors;
                foreach (var error in errors)
                    Log.Logger.ErrorException(error.ValidationErrors.ToString(), ex);

                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            foreach (var change in changes)
                OnAfterSetChanged(change.Key, change.Value);

            return result;
        }

        private int QuickSaveChanges()
        {
            return base.SaveChanges();
        }

        private static IEnumerable<KeyValuePair<Entity, EntityChangeState>> GetChangeset(IEnumerable<DbEntityEntry<Entity>> changeset)
        {
            if (changeset == null)
                return new KeyValuePair<Entity, EntityChangeState>[0];

            return
                from entry in changeset
                where
                    entry.State == EntityState.Added ||
                    entry.State == EntityState.Deleted ||
                    entry.State == EntityState.Modified
                select
                    new KeyValuePair<Entity, EntityChangeState>(entry.Entity, GetEntityChangeState(entry));

        }

        private static EntityChangeState GetEntityChangeState(DbEntityEntry<Entity> entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    return EntityChangeState.Add;

                case EntityState.Modified:
                    return EntityChangeState.Modify;

                case EntityState.Deleted:
                    return EntityChangeState.Delete;

                default:
                    throw new Exception();
            }
        }

        private static DbConnection GetNewConnection()
        {
            var connectionString = @"Data Source=" + Config.GetDbPath() + ";Version=3;";
            return new SQLiteConnection(connectionString);
        }

        public static void Truncate()
        {
            var tables = new List<string>();
            using (var connection = GetNewConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT name FROM sqlite_master WHERE type='table' AND NOT name LIKE 'sqlite_%';";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                }

                foreach (var table in tables)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"DELETE from " + table + @";";
                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }

            Vacuum();
        }

        public static void Sql(string sql)
        {
            using (var connection = GetNewConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public static void Vacuum()
        {
            Sql(@"VACUUM;");
        }

        #region IObservableSet members
        
        public event EventHandler<SetChangedEventArgument<Entity>> AfterSetChanged;

        public void OnAfterSetChanged(Entity entity, EntityChangeState state)
        {
            var handler = AfterSetChanged;
            if(handler != null)
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
    }
}