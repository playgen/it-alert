using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Core.Messaging.Extensions
{
	public static class MessageScopeExtensions
	{
		public static MessageScope RemoveScope(this MessageScope messageScope, MessageScope scopeToRemove)
		{
			return (messageScope ^ scopeToRemove) & messageScope;
		}

		public static bool HasScope(this MessageScope messageScope, MessageScope testScope)
		{
			return (messageScope & testScope) == testScope;
		}
	}
}
