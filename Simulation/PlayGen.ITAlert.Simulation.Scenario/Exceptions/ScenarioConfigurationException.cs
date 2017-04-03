using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Exceptions;

namespace PlayGen.ITAlert.Simulation.Scenario.Exceptions
{
	public class ScenarioConfigurationException : ConfigurationException
	{
		public ScenarioConfigurationException()
		{
		}

		public ScenarioConfigurationException(string message)
			: base(message)
		{
		}

		public ScenarioConfigurationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
