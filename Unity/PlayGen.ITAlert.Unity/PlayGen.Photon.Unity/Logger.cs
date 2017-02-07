using System;
using PlayGen.Photon.Messages.Logging;
using PlayGen.Photon.Unity.Messaging;

namespace PlayGen.Photon.Unity
{
	public static class Logger
	{
		private static Messenger Messenger;

		internal static int PlayerPhotonId { get; set; }

		public static void Log(LogLevel logLevel, string message)
		{
			if (message == null)
			{
				UnityLog(logLevel, message);
			}

			Messenger.SendMessage(new LogMessage()
			{
				PlayerPhotonId = PlayerPhotonId,
				LogLevel = logLevel,
				Message = message
			});
		}

		public static void LogFatal(string message)
		{
			Log(LogLevel.Fatal, message);
		}

		public static void LogFatal(Exception exception)
		{
			Log(LogLevel.Fatal, exception.Message);
		}

		public static void LogError(string message)
		{
			Log(LogLevel.Error, message);
		}

		public static void LogError(Exception exception)
		{
			Log(LogLevel.Error, exception.Message);
		}

		public static void LogWarn(string message)
		{
			Log(LogLevel.Warn, message);
		}
		public static void LogDebug(string message)
		{
			Log(LogLevel.Debug, message);
		}

		public static void LogInfo(string message)
		{
			Log(LogLevel.Info, message);
		}

		internal static void SetMessenger(Messenger messenger)
		{
			Messenger = messenger;
		}

		private static void UnityLog(LogLevel logLevel, string message)
		{
			switch (logLevel)
			{
				case LogLevel.Fatal:
				case LogLevel.Error:
					UnityEngine.Debug.LogError(message);
					break;
				case LogLevel.Warn:
					UnityEngine.Debug.LogWarning(message);
					break;
				case LogLevel.Debug:
				case LogLevel.Info:
					UnityEngine.Debug.Log(message);
					break;
				default:
					UnityEngine.Debug.LogError($"Unhandled log level type: {logLevel}. \nMessage: {message}");
					break;
			}
		}
	}
}