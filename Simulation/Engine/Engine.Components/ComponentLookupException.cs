using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Exceptions;

namespace Engine.Components
{
	public class ComponentLookupException : EngineException
	{
		public Type ComponentType { get; }

		public ComponentLookupException(string message, Type componentType) : base(message)
		{
			ComponentType = componentType;
		}

		public ComponentLookupException(string message, Exception innerException, Type componentType) : base(message, innerException)
		{
			ComponentType = componentType;
		}
	}
}
