using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.Events;

namespace Singularity
{
	public static class Logger
	{
		public static event EventHandler<LoggerEventArgs> OnLoggedMessage;

		public static void Log(object sender, LogLevel logLevel, String message)
		{
			OnLoggedMessage?.Invoke(sender, new LoggerEventArgs(logLevel, message));
		}

		public static void LogInformation(object sender, String message) => Log(sender, LogLevel.Information, message);
		public static void LogWarning(object sender, String message) => Log(sender, LogLevel.Warning, message);
		public static void LogError(object sender, String message) => Log(sender, LogLevel.Error, message);


	}
}
