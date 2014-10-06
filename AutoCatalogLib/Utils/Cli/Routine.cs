using System;

namespace AutoCatalogLib.Utils.Cli
{
    abstract class Routine
    {
        private ArgumentsSet _arguments;

        protected ArgumentsSet Arguments
        {
            get { return _arguments; }
        }

        public void SetArguments(ArgumentsSet arguments)
        {
            _arguments = arguments;
        }

        public abstract void Run();

        protected bool RequestAck(string message = @"")
        {
            if (message.Length > 0)
                message += @" ";

            Console.Write(message + @"Подтвердите (yes/no): ");
            var answer = Console.ReadLine().Trim();
            return (answer == "y" || answer == "yes");
        }
    }
}