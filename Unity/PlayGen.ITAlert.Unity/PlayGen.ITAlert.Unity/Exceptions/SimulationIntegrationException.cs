using System;

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
