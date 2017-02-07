using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class PlayerInventorySystem : ITickableSystem
	{
		private readonly ComponentMatcherGroup<Player, CurrentLocation, ItemStorage> _playerMatcher;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcher;

		public PlayerInventorySystem(IMatcherProvider matcherProvider)
			
		{
			_playerMatcher = matcherProvider.CreateMatcherGroup<Player, CurrentLocation, ItemStorage>();
			_subsystemMatcher = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
		}

		public void Tick(int currentTick)
		{
			var subsystems = _subsystemMatcher.MatchingEntityKeys;
			foreach (var playerTuple in _playerMatcher.MatchingEntities)
			{
				playerTuple.Component3.Items[0].Enabled = subsystems.Contains(playerTuple.Component2.Value);
			}
		}
	}
}
