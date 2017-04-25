using System.Collections.Generic;
using Engine.Archetypes;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Archetypes;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Modules.Resources.Components;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Modules.Resources.Systems
{
	/// <summary>
	/// This system is responsible for manipulating system resources, that is currently CPU and Memory // TODO: implement Bandwidth - this is currently only related to connection movement cost and has a discrete system
	/// Specific behaviours are implemented on classes implementing the ISubsystemResourceEffect interface
	/// TODO: evaluate wheteher each resource should have it's own system, or work on the extension pattern
	/// </summary>
	public class ResourcesSystem : ITickableSystem, IInitializingSystem
	{
		private readonly List<ISubsystemResourceEffect> _resourceEffects;


		public ResourcesSystem([InjectOptional] List<ISubsystemResourceEffect> resourceEffects) // TODO: remove zenject dependency when implicit optional collection paramters is implemented
		{
			_resourceEffects = resourceEffects;
		}

		public void Tick(int currentTick)
		{
			foreach (var resourceEffect in _resourceEffects)
			{
				resourceEffect.Tick();
			}
		}

		public void Initialize()
		{

		}

		public static void InitializeArchetypes()
		{
			// TODO: investigate whether this is a good way to extend archetypes in modules
			// this is a shit way to do it - since object references are persisted anything that inherits from a modified archetype is not updated
			Item.Archetype.HasComponent(new ComponentBinding<ConsumeMemory>()
			{
				ComponentTemplate = new ConsumeMemory()
				{
					Value = SimulationConstants.ItemMemoryConsumption,
				}
			});

			ConnectionNode.Archetype.HasComponent(new ComponentBinding<BandwidthResource>());

			Player.Archetype.HasComponent(new ComponentBinding<ConsumeCPU>()
			{
				ComponentTemplate = new ConsumeCPU()
				{
					Value = SimulationConstants.ActorCPUConsumption,
				}
			});
			SubsystemNode.Archetype.HasComponent(new ComponentBinding<MemoryResource>()
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

		public void Dispose()
		{
			// nothing to dispose
		}
	}
}
