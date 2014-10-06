using System.Text.RegularExpressions;

namespace AutoCatalogLib.Modules.Search
{
    [SingletonModule]
    internal class UpperCaseOnlyCharAndNumsStemmer : ITextStammer
    {
        private static readonly Regex FilterToRemovePattern = new Regex(@"[^a-zA-Zа-яёА-ЯЁ0-9]", 
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public string Stamm(string text)
        {
            return FilterToRemovePattern.Replace(text, @"").ToUpperInvariant();
        }
    }
}