using System;
using Engine.Components;
//using Engine.Messaging;
using Engine.Serialization;
using Engine.Util;

namespace Engine.Entities
{
	public delegate void EntityDelegate(Entity entity);

	public class Entity : //MessageHub
		ComponentContainer, IEquatable<Entity>
	{
		public int Id { get; protected set; }

		protected bool _disposed;

		protected EntityRegistry EntityRegistry { get; }

		#region constructors

		public Entity(EntityRegistry entityRegistry, ComponentRegistry componentRegistry)
			: base(componentRegistry)
		{
			EntityRegistry = entityRegistry;
		}

		#endregion

		public event EntityDelegate EntityDestroyed;

		public void Initialize()
		{
			Id = EntityRegistry.NextEntityId;
			_disposed = false;

			Components.Clear();
		}

		#region Event registry

		private void RaiseEntityDestroyed()
		{
			// TODO: reconsider using the event for the entity container
			EntityDestroyed?.Invoke(this);
			//OnNext(new EntityDestroyedMessage(MessageScope.External, this));
			EntityDestroyed = null;
		}

		public override void Dispose()
		{
			//TODO: interlock this perhaps, once we want to support multithreading
			if (_disposed == false)
			{
				_disposed = true;
				RaiseEntityDestroyed();
				base.Dispose();
			}
		}

		~Entity()
		{
			Dispose();
		}

		#endregion

		public bool Equals(Entity other)
		{
			return Id == other?.Id;
		}

		public override string ToString()
		{
			return $"Entity [{Id}] {this.GetType()}";
		}

		public virtual void OnDeserialized()
		{
		}
	}
}
