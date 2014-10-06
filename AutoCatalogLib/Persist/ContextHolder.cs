using System;

namespace AutoCatalogLib.Persist
{
    public class ContextHolder : IDisposable
    {
        private Context _contex;
        private bool _disposed;

        public Context Context
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("");

                if (_contex == null)
                    return _contex = new Context();

                if (!_contex.IsOutdated)
                    return _contex;

                _contex.Dispose();
                _contex = new Context();

                return _contex;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ContextHolder()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;

            if (disposing)
            {
                if (_contex != null)
                    _contex.Dispose();

                _contex = null;
            }
        }
    }
}