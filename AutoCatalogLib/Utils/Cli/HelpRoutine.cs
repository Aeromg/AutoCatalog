using System;
using System.Linq;

namespace AutoCatalogLib.Utils.Cli
{
    class HelpRoutine : Routine
    {
        private const string TokenFormat = "--{0}\n{1}\n";
        private const string UsageFormat = "»спользование:\t{0} --{1}";
        private const string UsageWithSubargumentsFormat = "»спользование:\t{0} --{1} {2}";
        private const string SubargumentWithValueUsageFormat = "--{0} {1}";
        private const string SubargumentWithoutValueUsageFormat = "--{0}";
        private const string UsageValueFormat = "<{0}>";
        private const string SubargumentFormat = "<--{0}>";
        private const string ArgumentValueUsageFormat = "\t\t{0} Ч {1}";
        private const string SubArgumentHeaderFormat = "\t\t--{0} Ч {1}";
        private const string SubArgumentUsageFormat = "\t\t{0}";
        private const string Conflicts = " онфликтует с:\t{0}";

        public override void Run()
        {
            Console.WriteLine(GetApplicationInfo());

            foreach (var header in ArgumentHeader.Headers.Where(h => !h.IsSubArgument))
            {
                Console.WriteLine(GetHeaderString(header));                                 // --arg Ч »м€ аргумента
                Console.WriteLine(GetUsage(header));                                        // »спользование: --arg 
                if(header.HasValue)
                    Console.WriteLine(GetValueUsage(header.Value));

                foreach (var subargument in header.SubArguments)
                {
                    Console.WriteLine(GetSubargumentHeader(subargument));
                    Console.WriteLine(GetSubargumentDetails(subargument));
                    if(subargument.HasValue)
                        Console.WriteLine(GetSubargumentValueUsage(subargument.Value));
                }

                if(header.HasConflicts)
                    Console.WriteLine(GetConflicts(header));

                Console.WriteLine();
            }
        }

        static string GetHeaderString(ArgumentHeader header)
        {
            return String.Format(TokenFormat, header.Token, header.Description);
        }

        static string GetUsage(ArgumentHeader header)
        {
            var argument = header.Token;
            var appfile = AppDomain.CurrentDomain.FriendlyName;
            if(header.HasValue)
                argument += @" " + String.Format(UsageValueFormat, header.Value.Name);

            if (!header.HasSubArgument)
                return String.Format(UsageFormat, appfile, argument);

            var subargs = String.Join(@" ", header.SubArguments.Select(GetSubargumentUsage));

            return String.Format(UsageWithSubargumentsFormat, appfile, argument, subargs);
        }

        static string GetConflicts(ArgumentHeader header)
        {
            var conflicts = String.Join(@", ", header.ConflictsWith.Select(c => c.Token));
            return String.Format(Conflicts, conflicts);
        }

        static string GetSubargumentUsage(ArgumentHeader subargument)
        {
            return subargument.HasValue
                ? String.Format(SubargumentFormat, subargument.Token + @" " + subargument.Value.Name)
                : String.Format(SubargumentFormat, subargument.Token);
        }

        static string GetValueUsage(ArgumentHeader.ArgumentValue value)
        {
            return String.Format(ArgumentValueUsageFormat, value.Name, value.Description);
        }

        static string GetSubargumentHeader(ArgumentHeader subargument)
        {
            return String.Format(SubArgumentHeaderFormat, subargument.Token, subargument.Description);
        }

        static string GetSubargumentUsageDetails(ArgumentHeader header)
        {
            if (header.HasValue)
                return String.Format(SubargumentWithValueUsageFormat, header.Token, header.Value.Name);

            return String.Format(SubargumentWithoutValueUsageFormat, header.Token);
        }

        static string GetSubargumentDetails(ArgumentHeader subargument)
        {
            return String.Format(SubArgumentUsageFormat, GetSubargumentUsageDetails(subargument));
        }

        static string GetSubargumentValueUsage(ArgumentHeader.ArgumentValue value)
        {
            return String.Format(SubArgumentUsageFormat, GetValueUsage(value));
        }

        static string GetApplicationInfo()
        {
            return @"—правка";
        }
    }
}