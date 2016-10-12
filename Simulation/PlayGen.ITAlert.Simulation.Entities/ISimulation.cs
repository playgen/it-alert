using System.Collections.Generic;
using PlayGen.Engine.Entities;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Entities.Visitors.Actors;
using PlayGen.ITAlert.Simulation.Entities.Visitors.Actors.Npc;
using PlayGen.ITAlert.Simulation.Entities.Visitors.Items;
using PlayGen.ITAlert.Simulation.Entities.World;
using PlayGen.ITAlert.Simulation.Entities.World.Systems;

namespace PlayGen.ITAlert.Simulation.Entities
{
	public interface ISimulation : IEntityRegistry
	{
		int CurrentTick { get; }

		#region entity factory

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		Subsystem CreateSubsystem(NodeConfig config);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subsystems">subsystems keyed by logical id</param>
		/// <param name="edgeConfig"></param>
		/// <returns></returns>
		Connection CreateConnection(Dictionary<int, Subsystem> subsystems, EdgeConfig edgeConfig);

		Player CreatePlayer(PlayerConfig playerConfig);

		IActor CreateNpc(NpcActorType type);

		IItem CreateItem(ItemType type);

		void SpawnVirus(int subsystemLogicalId);

		SimulationRules Rules { get; }

		#endregion
	}
}
