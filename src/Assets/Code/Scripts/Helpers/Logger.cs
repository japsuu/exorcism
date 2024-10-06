using System;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Helpers
{
    /// <summary>
    /// Log levels ranked by priority.
    /// </summary>
    public enum LogLevel
    {
        FATAL = 0,
        ERROR = 1,
        WARN = 2,
        INFO = 4,
        DEBUG = 8
    }
    
    /// <summary>
    /// Logger class for logging messages with different log levels.
    /// </summary>
    public static class Logger
    {
        public static LogLevel CurrentLogLevel { get; set; } = LogLevel.INFO;
        
        private const string INFO_COLOR = nameof(Color.white);
        private const string WARNING_COLOR = nameof(Color.yellow);
        private const string ERROR_COLOR = nameof(Color.red);


        [HideInCallstack]
        public static void Write(LogLevel logLevel, object message)
        {
            if(!IsLogLevelSufficient(logLevel))
                return;
            
            switch (logLevel)
            {
                case LogLevel.FATAL:
                    Debug.LogError(FormatMessage(ERROR_COLOR, message));
                    break;
                case LogLevel.ERROR:
                    Debug.LogError(FormatMessage(ERROR_COLOR, message));
                    break;
                case LogLevel.WARN:
                    Debug.LogWarning(FormatMessage(WARNING_COLOR, message));
                    break;
                case LogLevel.INFO:
                    Debug.Log(FormatMessage(INFO_COLOR, message));
                    break;
                case LogLevel.DEBUG:
                    Debug.Log(FormatMessage(INFO_COLOR, message));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }


        [HideInCallstack]
        public static void Write(LogLevel logLevel, string category, string message)
        {
            if(!IsLogLevelSufficient(logLevel))
                return;

            switch (logLevel)
            {
                case LogLevel.FATAL:
                    Debug.LogError(FormatMessageWithCategory(ERROR_COLOR, category, message));
                    break;
                case LogLevel.ERROR:
                    Debug.LogError(FormatMessageWithCategory(ERROR_COLOR, category, message));
                    break;
                case LogLevel.WARN:
                    Debug.LogWarning(FormatMessageWithCategory(WARNING_COLOR, category, message));
                    break;
                case LogLevel.INFO:
                    Debug.Log(FormatMessageWithCategory(INFO_COLOR, category, message));
                    break;
                case LogLevel.DEBUG:
                    Debug.Log(FormatMessageWithCategory(INFO_COLOR, category, message));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }


        [HideInCallstack]
        public static void Write(LogLevel logLevel, object message, Object context)
        {
            if(!IsLogLevelSufficient(logLevel))
                return;

            switch (logLevel)
            {
                case LogLevel.FATAL:
                    Debug.LogError(FormatMessage(ERROR_COLOR, message), context);
                    break;
                case LogLevel.ERROR:
                    Debug.LogError(FormatMessage(ERROR_COLOR, message), context);
                    break;
                case LogLevel.WARN:
                    Debug.LogWarning(FormatMessage(WARNING_COLOR, message), context);
                    break;
                case LogLevel.INFO:
                    Debug.Log(FormatMessage(INFO_COLOR, message), context);
                    break;
                case LogLevel.DEBUG:
                    Debug.Log(FormatMessage(INFO_COLOR, message), context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }


        [HideInCallstack]
        public static void Write(LogLevel logLevel, string category, string message, Object context)
        {
            if(!IsLogLevelSufficient(logLevel))
                return;

            switch (logLevel)
            {
                case LogLevel.FATAL:
                    Debug.LogError(FormatMessageWithCategory(ERROR_COLOR, category, message), context);
                    break;
                case LogLevel.ERROR:
                    Debug.LogError(FormatMessageWithCategory(ERROR_COLOR, category, message), context);
                    break;
                case LogLevel.WARN:
                    Debug.LogWarning(FormatMessageWithCategory(WARNING_COLOR, category, message), context);
                    break;
                case LogLevel.INFO:
                    Debug.Log(FormatMessageWithCategory(INFO_COLOR, category, message), context);
                    break;
                case LogLevel.DEBUG:
                    Debug.Log(FormatMessageWithCategory(INFO_COLOR, category, message), context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }


        [HideInCallstack]
        public static void LogFormat(LogLevel logLevel, string format, params object[] args)
        {
            if(!IsLogLevelSufficient(logLevel))
                return;

            switch (logLevel)
            {
                case LogLevel.FATAL:
                    Debug.LogError(FormatMessage(ERROR_COLOR, string.Format(format, args)));
                    break;
                case LogLevel.ERROR:
                    Debug.LogError(FormatMessage(ERROR_COLOR, string.Format(format, args)));
                    break;
                case LogLevel.WARN:
                    Debug.LogWarning(FormatMessage(WARNING_COLOR, string.Format(format, args)));
                    break;
                case LogLevel.INFO:
                    Debug.Log(FormatMessage(INFO_COLOR, string.Format(format, args)));
                    break;
                case LogLevel.DEBUG:
                    Debug.Log(FormatMessage(INFO_COLOR, string.Format(format, args)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }


        [HideInCallstack]
        public static void LogFormat(LogLevel logLevel, string category, string format, params object[] args)
        {
            if(!IsLogLevelSufficient(logLevel))
                return;

            switch (logLevel)
            {
                case LogLevel.FATAL:
                    Debug.LogError(FormatMessageWithCategory(ERROR_COLOR, category, string.Format(format, args)));
                    break;
                case LogLevel.ERROR:
                    Debug.LogError(FormatMessageWithCategory(ERROR_COLOR, category, string.Format(format, args)));
                    break;
                case LogLevel.WARN:
                    Debug.LogWarning(FormatMessageWithCategory(WARNING_COLOR, category, string.Format(format, args)));
                    break;
                case LogLevel.INFO:
                    Debug.Log(FormatMessageWithCategory(INFO_COLOR, category, string.Format(format, args)));
                    break;
                case LogLevel.DEBUG:
                    Debug.Log(FormatMessageWithCategory(INFO_COLOR, category, string.Format(format, args)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }


        private static string FormatMessage(string color, object message)
        {
            return $"<color={color}>{message}</color>";
        }


        private static string FormatMessageWithCategory(string color, string category, object message)
        {
            return $"<color={color}><b>[{category}]</b> {message}</color>";
        }


        private static bool IsLogLevelSufficient(LogLevel logLevel)
        {
            return logLevel <= CurrentLogLevel;
        }
    }
}