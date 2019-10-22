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
		private IEnumerable<ILogAppender> _logAppenders;

		public Log() : this(Array.Empty<ILogAppender>())
		{
		}

		public Log(params ILogAppender[] logAppenders)
		{
			_logAppenders = logAppenders;
			_stringBuilder = new StringBuilder();
		}

		public LogLevel LogLevel { get; set; }

		public void Write(LogLevel logLevel, string format, params object[] args)
		{
			if (logLevel > LogLevel)
				return;

			var formattedMessage = FormatMessage(string.Format(format, args), logLevel.ToString().ToUpperInvariant());

			foreach (var logAppender in _logAppenders)
				logAppender.WriteTo(logLevel, formattedMessage);

			_stringBuilder.Append(formattedMessage);
		}

		public void AddLogAppender(ILogAppender logAppender)
		{
			_logAppenders = _logAppenders.Concat(new[] {logAppender});
		}

		public override string ToString() => _stringBuilder.ToString();

		private static string FormatMessage(string message, string level) =>
			string.Format(CultureInfo.InvariantCulture, "{0} [{1:MM/dd/yy HH:mm:ss.fff}] {2}", level, DateTime.Now, message);
	}
}
