using System;
using PlayGen.Photon.Messages.Logging;

namespace PlayGen.Photon.Plugin.Extensions
{
	public static class LogMessageExtensions
	{
		public static NLog.LogLevel ToNLogLevel(this LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Fatal:
					return NLog.LogLevel.Fatal;

				case LogLevel.Error:
					return NLog.LogLevel.Error;
					
				case LogLevel.Warn:
					return NLog.LogLevel.Warn;

				case LogLevel.Debug:
					return NLog.LogLevel.Debug;

				case LogLevel.Info:
					return NLog.LogLevel.Info;

				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}
	}
}
