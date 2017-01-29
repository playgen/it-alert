using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;
using Engine.Systems;

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public interface IEnhancementSystemExtension : ISystemExtension
	{
		//TODO: push these down to the base interface, perhaps separate into sub interfaces corresponding to the system interfaces
		void OnSystemInitialize(Entity entity);

	}
}
