using System;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Visitors.Items;

namespace PlayGen.ITAlert.Simulation.World.Enhancements
{
	public class RepairSpawnManual : ManualEnhancement
	{
		#region constructors

		public RepairSpawnManual(ISimulation simulation, Subsystem subsystem) 
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

		protected override void OnItemActive(Subsystem subsystem)
		{
			if (Subsystem.CanAddItem())
			{
				var repair = Simulation.CreateItem(ItemType.Repair);
				if (Subsystem.TryAddItem(repair) == false)
				{
					repair.Dispose();
				}
			}
		}
	}
}
