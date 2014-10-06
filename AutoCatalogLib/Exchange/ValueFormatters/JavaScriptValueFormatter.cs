using System;
using System.Collections.Generic;
using System.Data;
using AutoCatalogLib.JavaScript;

namespace AutoCatalogLib.Exchange.ValueFormatters
{
    public class JavaScriptValueFormatter<TReturn> : JavaScriptValueFormatter, IFormatter<TReturn>
    {
        public override Type Type
        {
            get { return typeof (TReturn); }
            set { throw new ReadOnlyException(); }
        }

        public bool TryFormat(object source, out TReturn destination)
        {
            object tdest;
            var result = base.TryFormat(source, out tdest);
            destination = result ? (TReturn) tdest : default(TReturn);
            return result;
        }

        public new TReturn Format(object source)
        {
            return (TReturn) base.Format(source);
        }
    }

    public class JavaScriptValueFormatter : IFormatter, IDisposable
    {
        private readonly JavaScriptEnvironment _javascript = new JavaScriptEnvironment();

        public string Script
        {
            get { return _javascript.Script; }
            set { _javascript.Script = value; }
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual Type Type { get; set; }

        public Guid Guid { get; set; }

        public bool TryFormat(object source, out object destination)
        {
            try
            {
                destination = Format(source);
                return true;
            }
            catch (Exception)
            {
            }

            destination = Activator.CreateInstance(Type);
            return false;
        }

        public object Format(object source)
        {
            _javascript["input"] = source;
            _javascript.Run();

            return UnsafeChangeType(_javascript["output"], Type);
        }

        private static object UnsafeChangeType(object value, Type toType)
        {
            return toType.IsArray ? ConvertArray(value, toType.GetElementType()) : ConvertValue(value, toType);
        }

        private static object ConvertValue(object value, Type toType)
        {
            object result = Convert.ChangeType(value, toType);

            if (result != null && !result.GetType().IsAssignableFrom(toType))
                throw new Exception();

            return result;
        }

        private static object ConvertArray(object value, Type elementType)
        {
            var values = (object[]) value;
            var listType = typeof (List<>).MakeGenericType(new[] {elementType});
            var addMethod = listType.GetMethod("Add", new[] {elementType});
            var toArrayMethod = listType.GetMethod("ToArray", new Type[0]);
            var results = Activator.CreateInstance(listType);

            for (int i = 0; i < values.Length; i++)
                addMethod.Invoke(results, new[] {ConvertValue(i, elementType)});

            return toArrayMethod.Invoke(results, new object[0]);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JavaScriptValueFormatter()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_javascript != null)
                    _javascript.Dispose();
            }
        }
    }
}