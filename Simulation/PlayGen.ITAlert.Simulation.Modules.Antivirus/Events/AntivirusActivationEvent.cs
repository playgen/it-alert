using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using Engine.Systems.Scoring;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Events
{
	public class AntivirusActivationEvent : Event
	{
		public enum AntivirusActivationResult
		{
			Error = 0,
			NoVirusPresent,
			IncorrectGenome,
			SoloExtermination,
			CoopExtermination,
		}

		public int? PlayerEnttityId { get; set; }

		public AntivirusActivationResult ActivationResult { get; set; }

		public int LocationEntityId { get; set; }

		public int Uses { get; set; }
	}


}
