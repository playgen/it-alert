using System;
using Engine.Exceptions;

namespace Engine.Entities
{
	public class EntityRegistryException : EngineException
	{
		public EntityRegistryException()
		{
		}

		public EntityRegistryException(string message) : base(message)
		{
		}

		public EntityRegistryException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
