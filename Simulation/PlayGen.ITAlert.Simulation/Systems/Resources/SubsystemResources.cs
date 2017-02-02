﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components.Flags;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Systems.Resources
{
	public class SubsystemResources : ISystem, ITickableSystem
	{
		private readonly List<ISubsystemResourceEffect> _resourceEffects;


		public SubsystemResources([InjectOptional] List<ISubsystemResourceEffect> resourceEffects) // TODO: remove zenject dependency when implicit optional collection paramters is implemented
		{
			_resourceEffects = resourceEffects;
		}

		public void Tick(int currentTick)
		{
			foreach (var resourceEffect in _resourceEffects)
			{
				
			}
		}
	}
}
