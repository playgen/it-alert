using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using Engine.Systems;

namespace PlayGen.ITAlert.Simulation.Systems.Resources
{
	public interface ISubsystemResourceEffect : ISystemExtension
	{
		void Tick();
	}
}
