using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DacTools.Deployment.Core.Logging
{
	public sealed class Log : ILog
	{
		private readonly StringBuilder _stringBuilder;

		public IEnumerable<ILogAppender> LogAppenders { get; private set; }

		public Log() : this(Array.Empty<ILogAppender>())
		{
		}

		public Log(params ILogAppender[] logAppenders)
		{
			LogAppenders = logAppenders;
			_stringBuilder = new StringBuilder();
			LogLevel = LogLevel.Info;
		}

		public LogLevel LogLevel { get; set; }

		public void Write(LogLevel logLevel, string format, params object[] args)
		{
			if (logLevel > LogLevel)
				return;

			string formattedMessage = FormatMessage(string.Format(format, args), logLevel.ToString().ToUpperInvariant());

			foreach (var logAppender in LogAppenders)
				logAppender.WriteTo(logLevel, formattedMessage);

			_stringBuilder.Append(formattedMessage);
		}

		public void AddLogAppender(ILogAppender logAppender)
		{
			LogAppenders = LogAppenders.Concat(new[] {logAppender});
		}

		public override string ToString() => _stringBuilder.ToString();

		private static string FormatMessage(string message, string level) =>
			string.Format(CultureInfo.InvariantCulture, "{0} [{1:MM/dd/yy HH:mm:ss.fff}] {2}", level, DateTime.Now, message);
	}
}
