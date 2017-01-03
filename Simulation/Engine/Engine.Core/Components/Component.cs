using System;
using Engine.Entities;
//using Engine.Messaging;
using Engine.Util;

namespace Engine.Components
{
	public abstract class Component : //DelegatingObserver, 
		IComponent
	{
		private bool _disposed;

		public Entity Entity { get; private set; }

		#region disposal

		~Component()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			if (_disposed == false)
			{
				OnDispose();
				_disposed = true;
			}
		}

		protected virtual void OnDispose()
		{
			
		}

		#endregion

		public virtual void Initialize(Entity entity)
		{
			NotNullHelper.ArgumentNotNull(entity, nameof(entity));
			Entity = entity;
		}
	}
}
