using System;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public class RepairSpawnManual : ManualEnhancement
	{
		#region constructors

		public RepairSpawnManual(ISimulation simulation, System subsystem) 
			: base(simulation, EnhancementType.RepairSpawnManual, subsystem, 1)	// this has an active duration of one to make it one-shot
		{
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		public RepairSpawnManual()
		{
			
		}

		#endregion

		public override EnhancementState GenerateState()
		{
			return new EnhancementState(Id, EnhancementType)
			{
				Active = IsActive,
				ActiveDuration = ActiveDuration,
				ActiveTicksRemaining = ActiveTicksRemaining,
			};
		}

		protected override void OnItemActive(System subsystem)
		{
			if (System.CanAddItem())
			{
				var repair = Simulation.CreateItem(ItemType.Repair);
				if (System.TryAddItem(repair) == false)
				{
					repair.Dispose();
				}
			}
		}
	}
}
