using NLog;
using System;

namespace Asterius.Base
{
    public static class Log
    {
        public const string Name = "Asterius";

        public static ILogger Instance { get; set; }
        private static ILogger _Instance = null;
        private static ILogger _Forwarder
        {
            get
            {
                ILogger iLogger = Instance;
                if ((null == iLogger)
                    && (null == _Instance))
                {
                    _Instance = LogManager.GetLogger(
                        Name
                    );
                }
                return iLogger ?? _Instance;
            }
        }

        public static void Warning(string message, params object[] args)
        {
            _Forwarder.Warn(
                message,
                args
            );
        }

        public static void Info(string message, params object[] args)
        {
            _Forwarder.Info(
                message,
                args
            );
        }

        public static void Debug(string message, params object[] args)
        {
            _Forwarder.Debug(
                message,
                args
            );
        }

        public static void Error(string message, params object[] args)
        {
            _Forwarder.Error(
                message,
                args
            );
        }

        public static void Fatal(string message, params object[] args)
        {
            _Forwarder.Fatal(
                message,
                args
            );
        }

        public static void Trace(Exception exception)
        {
            _Forwarder.Trace(
                exception.ToString()
            );
        }

        public static void Warning(Exception exception)
        {
            _Forwarder.Warn(
                exception.ToString()
            );
        }

        public static void Info(Exception exception)
        {
            _Forwarder.Info(
                exception.ToString()
            );
        }

        public static void Debug(Exception exception)
        {
            _Forwarder.Debug(
                exception.ToString()
            );
        }

        public static void Error(Exception exception)
        {
            _Forwarder.Error(
                exception.ToString()
            );
        }

        public static void Fatal(Exception exception)
        {
            _Forwarder.Fatal(
                exception.ToString()
            );
        }
    }
}