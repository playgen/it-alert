using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using Engine.Components;
using Engine.Entities;
using Engine;
using Engine.Commands;
using Engine.Common;
using Engine.Exceptions;
using Engine.Planning;
using Engine.Serialization;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Layout;
using PlayGen.ITAlert.Simulation.Systems.Movement;
using Zenject;

namespace PlayGen.ITAlert.Simulation
{
	/// <summary>
	/// Simulation class
	/// Handles all autonomous functionality of the game
	/// </summary>
	public class Simulation : ECS<SimulationConfiguration>
	{
		//TODO: replace with some sort of global components - or make the simulation or rather 'graph' or something an entity with the layout components on it

		public Simulation(SimulationConfiguration configuration,
			IEntityRegistry entityRegistry, 
			IMatcherProvider matcherProvider, 
			ISystemRegistry systemRegistry,
			// TODO: remove zenject dependency when implicit optional collection paramters is implemented
			IEntityFactoryProvider entityFactoryProvider,
			CommandQueue commandQueue)
			: base(configuration, 
				  entityRegistry, 
				  matcherProvider, 
				  systemRegistry, 
				  entityFactoryProvider, 
				  commandQueue)
		{
		}
	}
}
