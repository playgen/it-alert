using System;
using System.Collections.Generic;
using System.Linq;
//using Engine.Messaging;

namespace Engine.Entities
{
	public class EntityDestroyedMessage : Message
	{
		public Entity Entity { get; private set; }

		public EntityDestroyedMessage(MessageScope scope, Entity entity) 
			: base(scope)
		{
			Entity = entity;
		}
	}
}
