using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Evaluators.Filters;
using PlayGen.ITAlert.Simulation.Scenario.Exceptions;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class ItemTypeIsInInventory<TItemType> : IEvaluator<Simulation, SimulationConfiguration>
		where TItemType : class, IItemType
	{
		private readonly int _playerId;
		private PlayerConfig _player;
		private readonly IEntityFilter<Simulation, SimulationConfiguration> _filter;

		private ComponentMatcherGroup<Player, ItemStorage> _playerMatcherGroup;
		private ComponentMatcherGroup<TItemType> _itemMatcherGroup;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="playerId"></param>
		/// <param name="filter"></param>
		public ItemTypeIsInInventory(int playerId = 0, IEntityFilter<Simulation, SimulationConfiguration> filter = null)
		{
			_playerId = playerId;
			_filter = filter;
		}

		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			if (configuration.TrySelectPlayer(_playerId, out _player) == false)
			{
				throw new ScenarioConfigurationException($"Player not found with id {_playerId}");
			}
			_playerMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<Player, ItemStorage>();
			_itemMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<TItemType>();
			_filter?.Initialize(ecs, configuration);
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			return _playerMatcherGroup.TryGetMatchingEntity(_player.EntityId, out var playerTuple)
				&& playerTuple.Component2.Items[0]?.Item != null
				&& _itemMatcherGroup.TryGetMatchingEntity(playerTuple.Component2.Items[0].Item.Value, out var itemTuple)
				&& (_filter == null || _filter.Evaluate(itemTuple.Entity));
		}

		public void Dispose()
		{
			_playerMatcherGroup?.Dispose();
			_itemMatcherGroup?.Dispose();
			_filter?.Dispose();
		}
	}
}
