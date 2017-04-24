using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using Engine.Systems.Scoring;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Events
{
	public class CaptureActivationEvent : Event
	{
		public enum CaptureActivationResult
		{
			Error = 0,
			NoVirusPresent,
			SampleCaptured,
		}

		public int? PlayerEnttityId { get; set; }

		public CaptureActivationResult ActivationResult { get; set; }

		public int LocationEntityId { get; set; }
	}
}
