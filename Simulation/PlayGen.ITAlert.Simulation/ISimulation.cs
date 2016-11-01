using System.Collections.Generic;
using Engine.Core.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Systems;
using PlayGen.ITAlert.Simulation.Visitors;

namespace PlayGen.ITAlert.Simulation
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
		ISystem CreateSystem(NodeConfig config);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subsystems">subsystems keyed by logical id</param>
		/// <param name="edgeConfig"></param>
		/// <returns></returns>
		IConnection CreateConnection(Dictionary<int, ISystem> subsystems, EdgeConfig edgeConfig);

		IPlayer CreatePlayer(PlayerConfig playerConfig);

		// TODO: reimplement
		IActor CreateNpc(NpcActorType type);

		IItem CreateItem(ItemType type);

		void SpawnVirus(int subsystemLogicalId);

		SimulationRules Rules { get; }

		#endregion
	}
}
