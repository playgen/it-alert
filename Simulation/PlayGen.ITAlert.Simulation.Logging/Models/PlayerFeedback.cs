using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Logging.Database.Model;

namespace PlayGen.ITAlert.Simulation.Logging.Models
{
	public class PlayerFeedback
	{
		public int Id { get; set; }

		public virtual Player Player { get; set; }

		public virtual Player RankedPlayer { get; set; }

		public int LeadershipRank { get; set; }

		public int CommunicationRank { get; set; }

		public int CooperationRank { get; set; }
	}
}
