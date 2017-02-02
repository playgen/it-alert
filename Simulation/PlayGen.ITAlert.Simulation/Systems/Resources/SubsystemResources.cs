using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Systems.Resources
{
	public class SubsystemResources : Engine.Systems.System
	{
		private readonly List<ISubsystemResourceEffect> _resourceEffects;


		public SubsystemResources(IMatcherProvider matcherProvider, 
			IEntityRegistry entityRegistry,
			// TODO: remove zenject dependency when implicit optional collection paramters is implemented
			[InjectOptional] List<ISubsystemResourceEffect> resourceEffects) 
			: base(matcherProvider, entityRegistry)
		{
			_resourceEffects = resourceEffects;
		}
	}
}
