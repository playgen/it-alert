using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Entities;
using Engine.Core.Messaging;

namespace Engine.Entities.Messages
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
