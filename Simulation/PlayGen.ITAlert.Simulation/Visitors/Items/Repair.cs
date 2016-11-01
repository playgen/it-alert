using System;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation.Visitors.Items
{
	public class Repair : Item
	{
		/// <summary>
		/// Health increment per tick
		/// </summary>
		public const int RepairIncrement = 5;

		/// <summary>
		/// Number of ticks to activate
		/// </summary>
		public const int RepairDuration = 20;

		#region constructors

		public Repair(ISimulation simulation) 
			: base(simulation, ItemType.Repair, simulation.Rules.RepairItemsConsumable, RepairDuration)
		{
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		public Repair()
		{
			
		}

		#endregion

		public override ItemState GenerateState()
		{
			return base.GenerateState();
		}

		protected override void OnItemActive()
		{
			var subsystem = CurrentNode as System;
			//TODO: reimplement
			//subsystem?.ModulateHealth(RepairIncrement);
		}
	}
}
