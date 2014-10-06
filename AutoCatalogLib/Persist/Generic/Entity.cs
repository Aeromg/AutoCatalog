using System;

namespace AutoCatalogLib.Persist.Generic
{
    public abstract class Entity
    {
        private WeakReference _contextReference;

        public long Id { get; private set; }

        public bool IsNew
        {
            get { return Id == 0; }
        }

        internal virtual void BeforeStateChanged(Context context, EntityChangeState state)
        {
        }

        internal virtual void AfterStateChanged(Context context, EntityChangeState state)
        {
            if (state == EntityChangeState.Loaded)
                _contextReference = new WeakReference(context);
        }

        public void AddOrAttach(Context context)
        {
            context.GetRepository(GetType()).AddOrAttach(this);
        }

        public void Remove(Context context)
        {
            context.GetRepository(GetType()).Remove(this);
        }

        public void Save()
        {
            if (_contextReference.IsAlive)
                GetContext().SaveChanges();
            else
                AddOrAttach().SaveChanges();
        }

        public Context AddOrAttach()
        {
            var context = GetContext();
            AddOrAttach(context);
            return context;
        }

        public Context Remove()
        {
            var context = GetContext();
            Remove(context);
            return context;
        }

        private Context GetContext()
        {
            return _contextReference != null && _contextReference.IsAlive ?
                (Context)_contextReference.Target : Context.Default;
        }
    }
}