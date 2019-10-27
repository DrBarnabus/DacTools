using System;
using System.Collections.Generic;

namespace DacTools.Deployment.Core.Logging
{
    public class ConsoleAppender : ILogAppender
    {
        private static readonly IDictionary<LogLevel, (ConsoleColor, ConsoleColor)> ColorPalette = CreateColorPalette();

        private readonly object _lock = new object();

        public void WriteTo(LogLevel logLevel, string message)
        {
            lock (_lock)
            {
                try
                {
                    var (backgroundColor, foregroundColor) = ColorPalette[logLevel];

                    Console.BackgroundColor = backgroundColor;
                    Console.ForegroundColor = foregroundColor;

                    if (logLevel == LogLevel.Error)
                        Console.Error.Write(message);
                    else if (logLevel != LogLevel.None)
                        Console.Write(message);
                }
                finally
                {
                    Console.ResetColor();

                    if (logLevel == LogLevel.Error)
                        Console.Error.WriteLine();
                    else if (logLevel != LogLevel.None)
                        Console.WriteLine();
                }
            }
        }

        private static IDictionary<LogLevel, (ConsoleColor, ConsoleColor)> CreateColorPalette() =>
            new Dictionary<LogLevel, (ConsoleColor, ConsoleColor)>
            {
                {LogLevel.Error, (ConsoleColor.DarkRed, ConsoleColor.White)},
                {LogLevel.Warn, (Console.BackgroundColor, ConsoleColor.Yellow)},
                {LogLevel.Info, (Console.BackgroundColor, ConsoleColor.White)},
                {LogLevel.Debug, (Console.BackgroundColor, ConsoleColor.DarkGray)}
            };
    }
}
