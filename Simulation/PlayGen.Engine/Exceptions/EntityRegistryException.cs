using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Exceptions
{
	public class EntityRegistryException : Exception
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
