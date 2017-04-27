using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;

namespace PlayGen.ITAlert.Simulation.Scoring.Team.Events
{
	public class TeamScoreEvent : Event
	{
		public int Score { get; set; }
	}
}
