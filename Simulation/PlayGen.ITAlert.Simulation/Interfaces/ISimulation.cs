using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Visitors.Actors;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Interfaces
{
	public interface ISimulation : IDisposable
	{
		int CurrentTick { get; }
		TEntity GetEntityById<TEntity>(int id) where TEntity : class, IEntity;

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

		int EntitySeed { get; }

		void OnDeserialized();

		SimulationRules Rules { get; }

		#endregion
	}
}
