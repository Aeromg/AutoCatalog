using System;
using System.Collections.Generic;

namespace AutoCatalogLib.Modules.Search
{
    public interface ISqlWhereClauseFormatter : IModule
    {
        string FormatContains(string column, string value);
        string FormatStartsWith(string column, string value);
        string FormatEquals(string column, string value);

        string Disjunction(IEnumerable<string> clauses);
        string Conjunction(IEnumerable<string> clauses);

        string Disjunction(string clause1, string clause2);
        string Conjunction(string clause1, string clause2);

        string Disjunction(params string[] clauses);
        string Conjunction(params string[] clauses);
    }
}
