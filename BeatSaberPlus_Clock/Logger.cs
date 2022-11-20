using System;

namespace BeatSaberPlus_Clock
{
    /// <summary>
    /// Logger instance holder
    /// </summary>
    internal class Logger
    {
        /// <summary>
        /// Logger instance
        /// </summary>
        internal static CP_SDK.Logging.ILogger Instance;
    }

    internal static class LoggerExtensions
    {
        internal static void Error(this CP_SDK.Logging.ILogger p_Logger, Exception p_Exception, string p_ClassName, string p_FunctionName)
        {
            p_Logger.Error($"[{p_ClassName}][{p_FunctionName}] : {p_Exception}");
        }
    }
}
