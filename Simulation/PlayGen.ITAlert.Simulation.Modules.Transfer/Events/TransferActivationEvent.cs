using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.Modules.Transfer.Events
{
	public class TransferActivationEvent : Event, IPlayerEvent, ISubsystemEvent
	{
		public enum TransferActivationResult
		{
			Error = 0,
			NoItemsPresent,
			PulledItem,
			PushedItem,
			SwappedItems,
		}

		public TransferActivationResult ActivationResult { get; set; }

		public int SubsystemEntityId { get; set; }

		public int? LocalItemEntityId { get; set; }

		public int? RemoteItemEntityId { get; set; }
		public int PlayerEntityId { get; set; }
	}
}
