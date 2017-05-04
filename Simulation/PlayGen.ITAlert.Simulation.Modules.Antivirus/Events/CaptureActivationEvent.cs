using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Events
{
	public class CaptureActivationEvent : Event, IPlayerEvent, ISubsystemEvent
	{
		public enum CaptureActivationResult
		{
			Error = 0,
			NoVirusPresent,
			SimpleGenomeCaptured,
			ComplexGenomeCaptured,
		}

		public CaptureActivationResult ActivationResult { get; set; }

		public int SubsystemEntityId { get; set; }
		public int PlayerEntityId { get; set; }
	}
}
