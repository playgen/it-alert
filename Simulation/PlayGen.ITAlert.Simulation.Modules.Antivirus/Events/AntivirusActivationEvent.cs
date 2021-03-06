﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Events
{
	public class AntivirusActivationEvent : Event, IPlayerEvent, ISubsystemEvent
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

		public int SubsystemEntityId { get; set; }

		public int Uses { get; set; }

		public int GenomeEradicated { get; set; }

		public Dictionary<int, int> MalwareCount { get; set; }

		public int PlayerEntityId { get; set; }

	}


}
