using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.Scoring.Player.Events
{
	public class PlayerScoreEvent : Event, IPlayerEvent
	{
		public int ResourceManagement { get; set; }

		public int Systematicity { get; set; }

		public int PlayerEntityId { get; set; }
	}
}
