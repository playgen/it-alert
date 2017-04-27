using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Events
{
	public class AntivirusActivationEvent : PlayerEvent
	{
		public enum AntivirusActivationResult
		{
			Error = 0,
			NoVirusPresent,
			IncorrectGenome,
			SoloExtermination,
			CoopExtermination,
		}

		public AntivirusActivationResult ActivationResult { get; set; }

		public int LocationEntityId { get; set; }

		public int Uses { get; set; }
	}


}
