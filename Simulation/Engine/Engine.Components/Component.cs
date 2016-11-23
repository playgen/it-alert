﻿using System;
using Engine.Components.Messaging;
using Engine.Core.Components;
using Engine.Core.Entities;
using Engine.Core.Messaging;
using Engine.Core.Util;

namespace Engine.Components
{
	public abstract class Component : DelegatingObserver, IComponent
	{
		protected IEntity Entity { get; private set; }

		#region constructor

		protected Component(IEntity entity)
		{
			NotNullHelper.ArgumentNotNull(entity, nameof(entity));
			Entity = entity;
		}

		#endregion

		#region disposal

		~Component()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
		}

		#endregion
	}
}