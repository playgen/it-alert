using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Components.Resources;

namespace PlayGen.ITAlert.Simulation.Systems.Resources
{
	// ReSharper disable once InconsistentNaming
	public class VisitorsConsumeCPUEffect : ISubsystemResourceEffect
	{
		private readonly IEntityRegistry _entityRegistry;

		public VisitorsConsumeCPUEffect(IEntityRegistry entityRegistry)
		{
			_entityRegistry = entityRegistry;
		}

		public void Tick(Entity subsystem)
		{
			CPUResource cpuResource;
			Visitors visitors;
			if (subsystem.TryGetComponent(out cpuResource)
				&& subsystem.TryGetComponent(out visitors))
			{
				var sum = 0;
				foreach (var visitorId in visitors.Value)
				{
					Entity visitor;
					if (_entityRegistry.TryGetEntityById(visitorId, out visitor))
					{
						ConsumeCPU consumeCpu;
						if (visitor.TryGetComponent(out consumeCpu))
						{
							sum += consumeCpu.Value;
						}
					}
				}
				cpuResource.SetValue(sum);
			}
		}
	}
}
