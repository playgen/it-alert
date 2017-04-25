using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using Engine.Systems.Scoring;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Events
{
	public class AnalyserActivationEvent : PlayerEvent
	{
		public enum AnalyserActivationResult
		{
			Error = 0,
			NoSamplePresent,
			OutputContainerFull,
			AnalysisComplete,
		}

		public AnalyserActivationResult ActivationResult { get; set; }

		public int? LocationEntityId { get; set; }

		public int GenomeProduced { get; set; }
	}
}
