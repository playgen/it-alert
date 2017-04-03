using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public interface IPlayerConfigFactory
	{
		PlayerConfig GetNextPlayerConfig(int index);
	}
}
