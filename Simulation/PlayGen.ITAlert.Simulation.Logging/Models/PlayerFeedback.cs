using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Logging.Database.Model;

namespace PlayGen.ITAlert.Simulation.Logging.Models
{
	public class PlayerFeedback
	{
		public Guid GameId { get; set; }
		public virtual GameInstance Game { get; set; }


		public int? PlayerId { get; set; }

		public virtual Player Player { get; set; }

		public int? RankedPlayerId { get; set; }

		public virtual Player RankedPlayer { get; set; }

		public int RankingCategory0 { get; set; }

		public int RankingCategory1 { get; set; }

		public int RankingCategory2 { get; set; }
	}
}
