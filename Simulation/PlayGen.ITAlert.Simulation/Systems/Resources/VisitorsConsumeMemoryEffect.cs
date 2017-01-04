using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Systems.Resources
{
	public class VisitorsConsumeMemoryEffect : ISubsystemResourceEffect
	{
		public void Tick(Entity subsystem)
		{
			MemoryResource memoryResource;
			Visitors visitors;
			if (subsystem.TryGetComponent(out memoryResource)
				&& subsystem.TryGetComponent(out visitors))
			{
				var sum = 0;
				foreach (var visitor in visitors.Value.Values)
				{
					ConsumeMemory consumeMemory;
					if (visitor.TryGetComponent(out consumeMemory))
					{
						sum += consumeMemory.Value;
					}
				}
				memoryResource.SetValue(sum);
			}
		}

	}
}
