using System;
using System.Collections.Generic;
using GameWork.Core.Logging;
using GameWork.Core.Logging.Loggers;
using PlayGen.Photon.Messages.Logging;
using PlayGen.Photon.Unity.Messaging;

namespace PlayGen.Photon.Unity
{
	public static class Logger
	{
		private static Messenger Messenger;
		private static bool _isSendingMessage;

		internal static int PlayerPhotonId { get; set; }

        public static void Log(LogType logType, string message)
		{
			if (Messenger == null || _isSendingMessage)
			{
				LogProxy.Log(logType, message);
				_isSendingMessage = false;
			}
			else
			{
				_isSendingMessage = true;
				Messenger.SendMessage(new LogMessage
										{
					PlayerPhotonId = PlayerPhotonId,
					LogType = logType,
					Message = message
				});
				_isSendingMessage = false;
			}
		}

		public static void LogFatal(string message)
		{
			Log(LogType.Fatal, message);
		}

		public static void LogFatal(Exception exception)
		{
			Log(LogType.Fatal, exception.Message);
		}

		public static void LogError(string message)
		{
			Log(LogType.Error, message);
		}

		public static void LogError(Exception exception)
		{
			Log(LogType.Error, exception.Message);
		}

		public static void LogWarn(string message)
		{
			Log(LogType.Warning, message);
		}
		public static void LogDebug(string message)
		{
			Log(LogType.Debug, message);
		}

		public static void LogInfo(string message)
		{
			Log(LogType.Info, message);
		}

		internal static void SetMessenger(Messenger messenger)
		{
			Messenger = messenger;
		}
	}
}