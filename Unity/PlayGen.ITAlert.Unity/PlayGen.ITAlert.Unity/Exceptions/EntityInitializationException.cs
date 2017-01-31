using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Unity.Exceptions
{
	public class EntityInitializationException : SimulationIntegrationException
	{
		public EntityInitializationException()
		{
		}

		public EntityInitializationException(string message)
			: base(message)
		{
		}

		public EntityInitializationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
