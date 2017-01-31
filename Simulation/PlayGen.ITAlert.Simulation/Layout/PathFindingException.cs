using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Exceptions;

namespace PlayGen.ITAlert.Simulation.Layout
{
	public class PathFindingException : SimulationException
	{
		public PathFindingException()
		{
		}

		public PathFindingException(string message) : base(message)
		{
		}

		public PathFindingException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
