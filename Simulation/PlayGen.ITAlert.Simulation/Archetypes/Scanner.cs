using System;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.VisitorsProperty.Actors.Npc;

namespace PlayGen.ITAlert.Simulation.VisitorsProperty.Items
{
	public class Scanner : Item
	{
		/// <summary>
		/// Number of ticks to activate
		/// </summary>
		private const int ScanDuration = 60;

		#region constructors

		public Scanner(ISimulation simulation) 
			: base(simulation, ItemType.Scanner, true, ScanDuration)
		{
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		public Scanner()
		{
			
		}

		#endregion

		public override ItemState GenerateState()
		{
			return base.GenerateState();
		}

		protected override void OnItemDeactivating()
		{
			var subsystem = CurrentNode as System;
			if (subsystem != null)
			{
				var infection = subsystem.GetVisitorOfType<IInfection>();
				infection?.SetVisible(true);
			}
		}
	}
}
