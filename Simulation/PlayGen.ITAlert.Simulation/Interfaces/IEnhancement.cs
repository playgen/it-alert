using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Interfaces
{
	public interface IEnhancement : IITAlertEntity
	{
		EnhancementType EnhancementType { get; }
	}
}
