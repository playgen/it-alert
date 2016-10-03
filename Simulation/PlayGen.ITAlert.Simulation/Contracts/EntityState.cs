using System.Collections.Generic;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	public class EntityState
	{
		public int Id { get; }

		public EntityType EntityType { get; }

		public EntityState(int id, EntityType entityType)
		{
			Id = id;
			EntityType = entityType;
		}
	}
}
