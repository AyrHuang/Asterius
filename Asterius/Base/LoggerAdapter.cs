using NLog;
using System;

namespace Asterius.Base
{
    // TODO: Consider a better way to accept out side NLog.ILogger Interface
    public static class LoggerAdapt
    {
        private readonly static ILogger InterfaceOfLogger  = LogManager.GetLogger("Logger");
        public static void Trace(string _message)
        {
            InterfaceOfLogger.Trace(
                _message
            );
        }

        public static void Warning(string _message)
        {
            InterfaceOfLogger.Warn(
                _message
            );
        }

        public static void Info(string _message)
        {
            InterfaceOfLogger.Info(
                _message
            );
        }

        public static void Debug(string _message)
        {
            InterfaceOfLogger.Debug(
                _message
            );
        }

        public static void Error(Exception _e)
        {
            InterfaceOfLogger.Error(
                _e.ToString()
            );
        }

        public static void Error(string _message)
        {
            InterfaceOfLogger.Error(
                _message
            );
        }

        public static void Fatal(Exception _e)
        {
            InterfaceOfLogger.Fatal(
                _e.ToString()
            );
        }

        public static void Fatal(string _message)
        {
            InterfaceOfLogger.Fatal(
                _message
            );
        }
    }
}
