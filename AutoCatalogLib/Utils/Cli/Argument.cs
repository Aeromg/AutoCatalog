using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoCatalogLib.Utils.Cli
{
    public class Argument
    {
        public abstract class ArgumentParserException : Exception { }

        public class UnexpectedArgumentException : ArgumentParserException
        {
            public string ArgumentName { get; private set; }

            public UnexpectedArgumentException(string argument)
            {
                ArgumentName = argument;
            }
        }

        static class Parser
        {
            private static readonly Regex PhraseRegex = new Regex(@"--.*?(?=(\s--\w|$))", RegexOptions.Compiled);
            private static readonly Regex ArgumentRegex = new Regex(@"--.*?(?=\s|$)", RegexOptions.Compiled);

            public static IEnumerable<Argument> Parse(string cliArguments)
            {
                var args = new List<Argument>();

                foreach (var phrase in GetPhrasesDictionary(cliArguments))
                {
                    ArgumentHeader header;
                    if (!ArgumentHeader.TryGetHeader(phrase.Key, out header))
                        throw new UnexpectedArgumentException(phrase.Key);

                    args.Add(new Argument(header, phrase.Value));
                }

                CheckIsValidArgumentSet(args);

                return args;
            }

            private static void CheckIsValidArgumentSet(IEnumerable<Argument> arguments)
            {
            }

            private static IEnumerable<KeyValuePair<string, string>> GetPhrasesDictionary(string cliArguments)
            {
                var phrases = PhraseRegex.Matches(cliArguments).Cast<Match>()
                    .Where(m => m.Length > 0)
                    .Select(p => p.Value);

                return phrases.Select(SplitPhrase).ToDictionary(p => p.Key, p => p.Value);
            }
            private static KeyValuePair<string, string> SplitPhrase(string phrase)
            {
                var argument = ArgumentRegex.Match(phrase).Value;
                var value = phrase.Length > argument.Length ? phrase.Substring(argument.Length).Trim() : String.Empty;

                argument = argument.Substring(2);   // remove leading "--"

                return new KeyValuePair<string, string>(argument, value);
            }
        }

        private readonly ArgumentHeader _header;
        public ArgumentHeader Header
        {
            get { return _header; }
        }

        private readonly string _value;
        public string Value
        {
            get { return _value; }
        }

        private Argument(ArgumentHeader header, string value)
        {
            _header = header;
            _value = value;
        }

        public static ArgumentsSet Parse(string cliArguments)
        {
            return new ArgumentsSet(Parser.Parse(cliArguments));
        }
    }
}
