using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using Engine.Systems.Scoring;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Events
{
	public class CaptureActivationEvent : PlayerEvent
	{
		public enum CaptureActivationResult
		{
			Error = 0,
			NoVirusPresent,
			SimpleGenomeCaptured,
			ComplexGenomeCaptured,
		}

		public CaptureActivationResult ActivationResult { get; set; }

		public int LocationEntityId { get; set; }
	}
}
