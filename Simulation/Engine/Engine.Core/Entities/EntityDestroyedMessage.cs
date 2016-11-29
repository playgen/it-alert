using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Messaging;

namespace Engine.Entities
{
	public class EntityDestroyedMessage : Message
	{
		public IEntity Entity { get; private set; }

		public EntityDestroyedMessage(MessageScope scope, IEntity entity) 
			: base(scope)
		{
			Entity = entity;
		}
	}
}
