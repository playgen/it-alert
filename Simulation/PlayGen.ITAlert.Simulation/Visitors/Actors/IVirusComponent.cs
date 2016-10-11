using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Components;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Visitors.Actors
{
	interface IVirusComponent : IComponent
	{
		void OnTickOnSubsystem(Virus virus, Subsystem subsystem);
	}
}
