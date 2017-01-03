using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Messaging.Extensions
{
	public static class MessageExtensions
	{
		public static bool ProcessExternalScope(this IMessage message)
		{
			return message.Scope.HasScope(MessageScope.External);
		}

		public static void ClearExternalScope(this IMessage message)
		{
			message.Scope = message.Scope.RemoveScope(MessageScope.External);
		}

		public static bool ProcessInternalScope(this IMessage message)
		{
			return message.Scope.HasScope(MessageScope.Internal);
		}
	}
}
