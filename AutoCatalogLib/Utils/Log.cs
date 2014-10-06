using System;
using NLog;

namespace AutoCatalogLib.Utils
{
    public static class Log
    {
        private static Logger _logger;

        public static Logger Logger
        {
            get { return _logger ?? (_logger = LogManager.GetCurrentClassLogger()); }
        }

        public static Exception Exception(Exception ex)
        {
            Logger.ErrorException(ex.Message + "\n" + ex.ToString(), ex);
            return ex;
        }
    }
}
