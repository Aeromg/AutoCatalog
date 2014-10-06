using System.Collections.Generic;

namespace AutoCatalogLib.Persist.Generic
{
    public class EntityEqualityComparer<T> : IEqualityComparer<T> where T : Entity
    {
        public bool Equals(T x, T y)
        {
            if (x == null || y == null)
                return false;

            if (x.GetType() != y.GetType())
                return false;

            if (x.IsNew || y.IsNew)
                return ReferenceEquals(x, y);

            return x.Id == y.Id;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}