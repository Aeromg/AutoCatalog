using System;

namespace AutoCatalogLib.Exchange.ValueFormatters
{
    internal class LambdaValueFormatter<TReturn> : IFormatter<TReturn>
    {
        private readonly string _description;
        private readonly Func<object, TReturn> _func;
        private readonly string _name;
        private readonly Guid _guid;

        public LambdaValueFormatter(Guid guid, string name, string description, Func<object, TReturn> func)
        {
            _guid = guid;
            _name = name;
            _description = description;
            _func = func;
        }

        public LambdaValueFormatter(Guid guid, string name, Func<object, TReturn> func)
            : this(guid, name, String.Empty, func)
        {
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }

        public Type Type
        {
            get { return typeof (TReturn); }
        }

        public Guid Guid { get { return _guid; } }

        public bool TryFormat(object source, out TReturn destination)
        {
            try
            {
                destination = Format(source);
                return true;
            }
            catch (Exception)
            {
                destination = default(TReturn);
                return false;
            }
        }

        public TReturn Format(object source)
        {
            return _func.Invoke(source);
        }

        public bool TryFormat(object source, out object destination)
        {
            TReturn tdest;
            bool result = TryFormat(source, out tdest);
            destination = result ? (object) tdest : null;
            return result;
        }

        object IFormatter.Format(object source)
        {
            return Format(source);
        }
    }
}