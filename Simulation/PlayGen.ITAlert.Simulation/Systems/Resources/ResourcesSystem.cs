using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Systems.Resources
{
	/// <summary>
	/// This system is responsible for manipulating system resources, that is currently CPU and Memory // TODO: implement Bandwidth - this is currently only related to connection movement cost and has a discrete system
	/// Specific behaviours are implemented on classes implementing the ISubsystemResourceEffect interface
	/// TODO: evaluate wheteher each resource should have it's own system, or work on the extension pattern
	/// </summary>
	public class ResourcesSystem : ISystem, ITickableSystem
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
	}
}
