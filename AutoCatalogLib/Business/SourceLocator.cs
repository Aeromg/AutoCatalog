using System;
using AutoCatalogLib.Exchange;

namespace AutoCatalogLib.Business
{
    public static class SourceLocator
    {
        public const string EmbeddedProtocol = @"blob:";
        public const string WebProtocol = @"url:";
        public const string LocalPathProtocol = @"local:";

        public static ISource GetSourceByLocation(string location)
        {
            var target = GetTargetLocation(location);

            if (IsLocalPathLocation(location))
                return new FileSource() { Location = location };

            if (IsFileEntityLocation(location))
                return new EmbeddedSource() { Location = location };

            throw new NotImplementedException(@"Источник " + location + @" не поддерживается");
        }

        public static string GetTargetLocation(string location)
        {
            return location.Substring(location.IndexOf(':') + 1);
        }

        public static bool IsFileEntityLocation(string location)
        {
            return location.StartsWith(EmbeddedProtocol);
        }

        public static bool IsWebLocation(string location)
        {
            return location.StartsWith(WebProtocol);
        }

        public static bool IsLocalPathLocation(string location)
        {
            return location.StartsWith(LocalPathProtocol);
        }
    }
}
