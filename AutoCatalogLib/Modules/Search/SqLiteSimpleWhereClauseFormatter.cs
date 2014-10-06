using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCatalogLib.Modules.Search
{
    [SingletonModule]
    internal class SqLiteSimpleWhereClauseFormatter : ISqlWhereClauseFormatter
    {
        public string FormatContains(string column, string value)
        {
            return column + " LIKE '%" + value + "%'";
        }

        public string FormatStartsWith(string column, string value)
        {
            return column + " LIKE '" + value + "%'";
        }

        public string FormatEquals(string column, string value)
        {
            return column + " = '" + value + "'";
        }

        public string Disjunction(IEnumerable<string> clauses)
        {
            var junction = String.Join(" OR ", clauses);
            return junction.Length > 0 ? "(" + junction + ")" : "0";
        }

        public string Conjunction(IEnumerable<string> clauses)
        {
            var junction = String.Join(" AND ", clauses);
            return junction.Length > 0 ? "(" + junction + ")" : "0";
        }

        public string Disjunction(string clause1, string clause2)
        {
            return "(" + clause1 + " OR " + clause2 + ")";
        }

        public string Conjunction(string clause1, string clause2)
        {
            return "(" + clause1 + " AND " + clause2 + ")";
        }

        public string Disjunction(params string[] clauses)
        {
            return String.Join(" OR ", (IEnumerable<string>)clauses);
        }

        public string Conjunction(params string[] clauses)
        {
            return String.Join(" AND ", (IEnumerable<string>)clauses);
        }
    }
}
