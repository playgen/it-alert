using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Simulation
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
