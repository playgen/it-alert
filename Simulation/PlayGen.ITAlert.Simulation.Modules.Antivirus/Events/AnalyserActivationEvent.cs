using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using Engine.Systems.Scoring;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Events
{
	public class AnalyserActivationEvent : Event
	{
		public enum AnalyserActivationResult
		{
			Error = 0,
			NoSamplePresent,
			OutputContainerFull,
			AnalysisComplete,
		}

		public int? PlayerEnttityId { get; set; }

		public AnalyserActivationResult ActivationResult { get; set; }

		public int? LocationEntityId { get; set; }

		public int GenomeProduced { get; set; }
	}
}
