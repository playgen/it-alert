using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Entities;
using Engine.Core.Messaging;

namespace Engine.Entities.Messages
{
	public class EntityDestroyedMessage : IMessage
	{
		public IEntity Entity { get; private set; }

		public EntityDestroyedMessage(IEntity entity)
		{
			Entity = entity;
		}
	}
}
