using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Unity.Exceptions
{
	public class SimulationSynchronisationException : Exception
	{
		public SimulationSynchronisationException()
		{
		}

		public SimulationSynchronisationException(string message) : base(message)
		{
		}

		public SimulationSynchronisationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
