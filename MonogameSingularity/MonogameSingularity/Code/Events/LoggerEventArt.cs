using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.Events
{
	public class LoggerEventArgs : EventArgs
	{
		public LogLevel Level { get; }

		public String Message { get; }

		internal LoggerEventArgs(LogLevel level, String message)
		{
			this.Level = level;
			this.Message = message;
		}

	}
}
