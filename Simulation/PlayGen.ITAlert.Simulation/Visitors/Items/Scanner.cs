using System;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Visitors.Actors;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Visitors.Items
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
			var subsystem = CurrentNode as Subsystem;
			if (subsystem != null)
			{
				var infection = subsystem.GetVisitorOfType<IInfection>();
				infection?.SetVisible(true);
			}
		}
	}
}
