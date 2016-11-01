using System;
using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public abstract class SystemEnhancement : ITAlertEntity<EnhancementState>, IEnhancement
	{
		[SyncState(StateLevel.Setup)]
		protected System System { get; set; }

		[SyncState(StateLevel.Setup)]
		public EnhancementType EnhancementType { get; protected set; }

		#region constructors

		/// <summary>
		/// 
		/// </summary>
		/// <param name="simulation"></param>
		/// <param name="enhancementType"></param>
		/// <param name="subsystem"></param>
		protected SystemEnhancement(ISimulation simulation, EnhancementType enhancementType, System subsystem)
			: base(simulation, EntityType.Enhancement)
		{
			System = subsystem;
			EnhancementType = enhancementType;
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected SystemEnhancement()
		{
			
		}

		#endregion
	}
}
