using System;

namespace PlayGen.ITAlert.Simulation.Exceptions
{
	public class SimulationException : Exception
	{
		public SimulationException()
		{
		}

		public SimulationException(string message) : base(message)
		{
		}

		public SimulationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
