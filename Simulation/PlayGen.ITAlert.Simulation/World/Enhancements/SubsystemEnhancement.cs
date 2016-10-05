using System;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Interfaces;

namespace PlayGen.ITAlert.Simulation.World.Enhancements
{
	public abstract class SubsystemEnhancement : ITAlertEntity<EnhancementState>, IEnhancement
	{
		[SyncState(StateLevel.Setup)]
		protected Subsystem Subsystem { get; set; }

		[SyncState(StateLevel.Setup)]
		public EnhancementType EnhancementType { get; protected set; }

		#region constructors

		/// <summary>
		/// 
		/// </summary>
		/// <param name="simulation"></param>
		/// <param name="enhancementType"></param>
		/// <param name="subsystem"></param>
		protected SubsystemEnhancement(ISimulation simulation, EnhancementType enhancementType, Subsystem subsystem)
			: base(simulation, EntityType.Enhancement)
		{
			Subsystem = subsystem;
			EnhancementType = enhancementType;
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected SubsystemEnhancement()
		{
			
		}

		#endregion
	}
}
