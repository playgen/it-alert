using System;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Interfaces;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Visitors.Items
{
	public class Cleaner : Item
	{
		/// <summary>
		/// Number of ticks to activate
		/// </summary>
		private const int ScanDuration = 60;

		#region constructors

		public Cleaner(ISimulation simulation) 
			: base(simulation, ItemType.Cleaner, true, ScanDuration)
		{
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		public Cleaner()
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
				if (infection?.Visible ?? false)
				{
					infection?.Dispose();
				}
			}
		}
	}
}
