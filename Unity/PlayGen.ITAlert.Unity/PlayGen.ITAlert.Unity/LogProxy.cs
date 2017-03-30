using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PlayGen.ITAlert.Unity
{
	public static class LogProxy
	{
		public static LogType LogLevel
		{
			get { return Logger.filterLogType; }
			set { Logger.filterLogType = value; }
		}

		public static ILogger Logger { get; set; } = UnityEngine.Debug.logger;

		public static void Exception(Exception exception)
		{
			Logger.Log(LogType.Exception, exception);
		}

		public static void Error(string message)
		{
			Logger.Log(LogType.Error, message);
		}

		public static void Warning(string message)
		{
			Logger.Log(LogType.Warning, message);
		}

		public static void Info(string message)
		{
			Logger.Log(LogType.Log, message);
		}

		public static void Assert(string message)
		{
			Logger.Log(LogType.Assert, message);
		}

	}
}
