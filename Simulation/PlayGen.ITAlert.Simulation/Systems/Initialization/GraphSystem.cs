using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Exceptions;

namespace PlayGen.ITAlert.Simulation.Systems.Initialization
{
	public class GraphSystem : IInitializingSystem
	{
		private readonly SimulationConfiguration _configuration;
		private readonly IEntityFactoryProvider _entityFactoryProvider;

		public Dictionary<int, Entity> Subsystems { get; private set; }
		

		public GraphSystem(SimulationConfiguration configuration,
			IEntityFactoryProvider entityFactoryProvider)
		{
			_configuration = configuration;
			_entityFactoryProvider = entityFactoryProvider;
		}

		#region Implementation of IInitializingSystem

		public void Initialize()
		{
			Subsystems = CreateSystems(_configuration.NodeConfiguration);
			var connections = CreateConnections(Subsystems, _configuration.EdgeConfiguration);
		}

		#endregion

		public Dictionary<int, Entity> CreateSystems(IEnumerable<NodeConfig> nodeConfigs)
		{
			return nodeConfigs.ToDictionary(sc => sc.Id, CreateSystem);
		}


		public Entity CreateSystem(NodeConfig config)
		{
			if (_entityFactoryProvider.TryCreateEntityFromArchetype(config.Archetype, out var subsystem))
			{
				config.EntityId = subsystem.Id;

				subsystem.GetComponent<Coordinate2DProperty>().X = config.X;
				subsystem.GetComponent<Coordinate2DProperty>().Y = config.Y;
				subsystem.GetComponent<Name>().Value = config.Name;

				return subsystem;
			}
			subsystem?.Dispose();

			throw new SimulationException($"Could not create system for archetype '{config.Archetype}'");
		}

		public List<Entity> CreateConnections(Dictionary<int, Entity> subsystems, IEnumerable<EdgeConfig> edgeConfigs)
		{
			return edgeConfigs.Select(cc => CreateConnection(subsystems, cc)).ToList();
		}

		public Entity CreateConnection(Dictionary<int, Entity> subsystems, EdgeConfig edgeConfig)
		{
			if (_entityFactoryProvider.TryCreateEntityFromArchetype(edgeConfig.Archetype, out var connection))
			{
				var head = subsystems[edgeConfig.Source];
				var tail = subsystems[edgeConfig.Destination];

				connection.GetComponent<MovementCost>().Value = edgeConfig.Weight;

				connection.GetComponent<GraphNode>().EntrancePositions.Add(head.Id, 0);
				connection.GetComponent<GraphNode>().ExitPositions.Add(tail.Id, SimulationConstants.ConnectionPositions * edgeConfig.Length);

				head.GetComponent<GraphNode>().ExitPositions.Add(connection.Id, edgeConfig.SourcePosition.ToPosition(SimulationConstants.SubsystemPositions));
				tail.GetComponent<GraphNode>().EntrancePositions.Add(connection.Id, edgeConfig.SourcePosition.OppositePosition().ToPosition(SimulationConstants.SubsystemPositions));

				edgeConfig.EntityId = connection.Id;
				return connection;
			}
			connection?.Dispose();
			throw new SimulationException($"Could not create connection from archetype '{edgeConfig.Archetype}'");
		}

	}
}
