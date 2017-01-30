using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Unity.Exceptions
{
	public class SimulationIntegrationException : Exception
	{
		public SimulationIntegrationException()
		{
		}

		public SimulationIntegrationException(string message) : base(message)
		{
		}

		public SimulationIntegrationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
