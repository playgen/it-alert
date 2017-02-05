using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Exceptions;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public class MovementException : SimulationException
	{
		public MovementException()
		{
		}

		public MovementException(string message) : base(message)
		{
		}

		public MovementException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
