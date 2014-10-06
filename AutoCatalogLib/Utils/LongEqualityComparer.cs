using System.Collections.Generic;

namespace AutoCatalogLib.Utils
{
    public class LongEqualityComparer : IEqualityComparer<long>
    {
        public bool Equals(long x, long y)
        {
            return x == y;
        }

        public int GetHashCode(long obj)
        {
            return obj.GetHashCode();
        }
    }
}
