using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class EntityConfig
	{
		public const int IdUnassigned = -1;

		/// <summary>
		/// Logical Id, order in config
		/// </summary>
		public int Id { get; set; } = IdUnassigned;

		/// <summary>
		/// Entity id in ECS
		/// </summary>
		public int EntityId { get; set; }
		
		public string Archetype { get; set; }
	}
}
