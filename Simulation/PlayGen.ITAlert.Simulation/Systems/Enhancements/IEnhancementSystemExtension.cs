using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public interface IEnhancementSystemExtension
	{
		void Initialize(Entity entity);
	}
}
