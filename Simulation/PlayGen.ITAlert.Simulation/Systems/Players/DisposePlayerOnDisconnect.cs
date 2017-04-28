using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Systems.Players
{
	public class DisposePlayerOnDisconnect : IPlayerSystemBehaviour
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Visitors> _visitorsMatcherGroup;

		public DisposePlayerOnDisconnect(IMatcherProvider matcherProvider)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_visitorsMatcherGroup = matcherProvider.CreateMatcherGroup<Visitors>();
		}

		public void OnPlayerJoined(int playerExternalId)
		{
			// nothing to do here
		}

		public void OnPlayerDisconnected(int playerEntityId)
		{
			if (_playerMatcherGroup.TryGetMatchingEntity(playerEntityId, out var playerTuple)
				&& playerTuple.Component3.Value.HasValue
				&& _visitorsMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value, out var visitorsTuple))
			{
				visitorsTuple.Component1.Values.Remove(playerEntityId);
				playerTuple.Entity.Dispose();
			}
		}


		public void Dispose()
		{
			_playerMatcherGroup?.Dispose();
			_visitorsMatcherGroup?.Dispose();
		}
	}
}
