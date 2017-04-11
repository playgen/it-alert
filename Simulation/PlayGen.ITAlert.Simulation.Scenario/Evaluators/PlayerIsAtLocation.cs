using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Evaluators;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Scenario.Configuration;
using PlayGen.ITAlert.Simulation.Scenario.Exceptions;
using PlayGen.ITAlert.Simulation.Systems.Players;

namespace PlayGen.ITAlert.Simulation.Scenario.Evaluators
{
	public class PlayerIsAtLocation : IEvaluator<Simulation, SimulationConfiguration>
	{
		private readonly int _playerId;

		private readonly int _nodeId;
		private NodeConfig _node;

		private ComponentMatcherGroup<Player, CurrentLocation> _playerMatcherGroup;

		private PlayerSystem _playerSystem;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nodeId"></param>
		/// <param name="playerId"></param>
		public PlayerIsAtLocation(int nodeId, int playerId = 0)
		{
			_nodeId = nodeId;
			_playerId = playerId;
		}

		public void Initialize(Simulation ecs, SimulationConfiguration configuration)
		{
			if (configuration.TrySelectNode(_nodeId, out _node) == false)
			{
				throw new ScenarioConfigurationException($"Node not found with id {_nodeId}");
			}

			if (ecs.TryGetSystem(out _playerSystem) == false)
			{
				throw new SimulationException("Unable to get PlayerSystem for Evaluation");
			}

			_playerMatcherGroup = ecs.MatcherProvider.CreateMatcherGroup<Player, CurrentLocation>();
		}

		public bool Evaluate(Simulation ecs, SimulationConfiguration configuration)
		{
			return _playerSystem.TryGetPlayerEntityId(_playerId, out var playerEntityId)
				&& _playerMatcherGroup.TryGetMatchingEntity(playerEntityId, out var playerTuple)
				&& playerTuple.Component2.Value == _node.EntityId;
		}
		public void Dispose()
		{
			_playerMatcherGroup?.Dispose();
		}
	}
}
