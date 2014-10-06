using System;
using System.Collections.Generic;
using Noesis.Javascript;

namespace AutoCatalogLib.JavaScript
{
    public sealed class JavaScriptEnvironment : IJavaScriptEnvironment
    {
        private readonly IDictionary<string, object> _defaultParameters = new Dictionary<string, object>();
        private JavascriptContext _context = new JavascriptContext();

        public string Script { get; set; }

        public IDictionary<string, object> DefaultParameters
        {
            get { return _defaultParameters; }
        }

        public void Run()
        {
            SetDefaultParameters();
            _context.Run(Script);
        }

        public void Reset()
        {
            if (_context == null)
                return;

            _context.TerminateExecution();
            _context.Dispose();

            _context = new JavascriptContext();
        }

        public object this[string parameter]
        {
            get { return _context.GetParameter(parameter); }
            set { _context.SetParameter(parameter, value); }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JavaScriptEnvironment()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context == null)
                    return;

                _context.TerminateExecution();
                _context.Dispose();

                _context = null;
            }
        }

        private void SetDefaultParameters()
        {
            foreach (var parameter in DefaultParameters)
                if (_context.GetParameter(parameter.Key) == null)
                    _context.SetParameter(parameter.Key, parameter.Value);
        }
    }
}