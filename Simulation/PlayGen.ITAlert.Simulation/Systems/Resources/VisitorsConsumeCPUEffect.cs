using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Systems.Resources
{
	public class VisitorsConsumeCPUEffect : ISubsystemResourceEffect
	{
		public void Tick(Entity subsystem)
		{
			CPUResource cpuResource;
			Visitors visitors;
			if (subsystem.TryGetComponent(out cpuResource)
				&& subsystem.TryGetComponent(out visitors))
			{
				var sum = 0;
				foreach (var visitor in visitors.Value.Values)
				{
					ConsumeCPU consumeCpu;
					if (visitor.TryGetComponent(out consumeCpu))
					{
						sum += consumeCpu.Value;
					}
				}
				cpuResource.SetValue(sum);
			}
		}
	}
}
