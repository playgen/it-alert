using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Resources;

namespace PlayGen.ITAlert.Simulation.Archetypes
{
	/// <summary>
	/// Archetype definition for subsystem entities
	/// </summary>
	public static class Subsystem
	{
		public static readonly Archetype Archetype = new Archetype("Subsystem")
			.Extends(Node.Archetype)
			.HasComponent(new ComponentBinding<Components.EntityTypes.Subsystem>())
			.HasComponent(new ComponentBinding<Name>())
			.HasComponent(new ComponentBinding<Coordinate2DProperty>())
			.HasComponent(new ComponentBinding<ItemActivator>())
			.HasComponent(new ComponentBinding<ItemStorage>()
			{
				ComponentTemplate = new ItemStorage()
				{
					Items = new ItemContainer[]
					{
						new ItemContainer()
						{
							Enabled = true,
						},
						new ItemContainer()
						{
							Enabled = true,
						},
						new ItemContainer()
						{
							Enabled = true,
						},
						new ItemContainer()
						{
							Enabled = true,
						},
					},
				}
			})
			.HasComponent(new ComponentBinding<MemoryResource>()
			{
				ComponentTemplate = new MemoryResource()
				{
					Value = SimulationConstants.SubsystemInitialMemory,
					Maximum = SimulationConstants.SubsystemMaxMemory,
				}
			})
			.HasComponent(new ComponentBinding<CPUResource>()
			{
				ComponentTemplate = new CPUResource()
				{
					Value = SimulationConstants.SubsystemInitialCPU,
					Maximum = SimulationConstants.SubsystemMaxCPU,
				}
			});
	}
}
