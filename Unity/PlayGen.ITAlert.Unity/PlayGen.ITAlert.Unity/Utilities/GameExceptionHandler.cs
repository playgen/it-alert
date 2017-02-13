using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayGen.ITAlert.Unity.Utilities
{
	public static class GameExceptionHandler
	{
		private static readonly List<Type> _ignoreExceptionTypes = new List<Type>();

		public static Exception Exception { get; private set; }
		public static bool HasException => Exception != null;

		public static event Action HadUnignoredExceptionEvent;

		public static void AddExceptionTypeToIgnore(params Type[] ignoreExceptionTypes)
		{
			_ignoreExceptionTypes.AddRange(ignoreExceptionTypes);
		}

		public static void ClearException()
		{
			Exception = null;
		}

		public static void OnException(Exception exception)
		{
			var exceptionType = exception.GetType();
			if (_ignoreExceptionTypes.All(type => type != exceptionType))
			{
				Exception = exception;
				HadUnignoredExceptionEvent?.Invoke();
			}
		}
	}
}