using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Exceptions
{
	public class EngineException : Exception
	{
		public EngineException()
		{
		}

		public EngineException(string message) : base(message)
		{
		}

		public EngineException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
