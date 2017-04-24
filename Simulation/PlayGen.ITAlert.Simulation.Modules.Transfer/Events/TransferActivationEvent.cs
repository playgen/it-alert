using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;

namespace PlayGen.ITAlert.Simulation.Modules.Transfer.Events
{
	public class TransferActivationEvent : Event
	{
		public enum TransferActivationResult
		{
			Error = 0,
			NoItemsPresent,
			PulledItem,
			PushedItem,
			SwappedItems,
		}

		public int? PlayerEnttityId { get; set; }

		public TransferActivationResult ActivationResult { get; set; }

		public int? LocationEntityId { get; set; }

		public int? LocalItemEntityId { get; set; }

		public int? RemoteItemEntityId { get; set; }
	}
}
