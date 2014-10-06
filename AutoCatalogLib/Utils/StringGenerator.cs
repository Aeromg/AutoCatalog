using System;
using System.Linq;

namespace AutoCatalogLib.Utils
{
    public static class StringGenerator
    {
        private static readonly char[] Alphabet = "abcdefghijklmnopqrstuvwxyz012345678".ToCharArray();
        private static readonly Random Generator = new Random();

        public static string GetRandomString(int length)
        {
            var result = new char[length];

            foreach (var i in Enumerable.Range(0, length))
                result[i] = Alphabet[Generator.Next(Alphabet.Length - 1)];

            return new string(result);
        }
    }
}
