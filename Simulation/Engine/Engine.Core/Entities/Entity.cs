using System;
using Engine.Messaging;
using Engine.Serialization;
using Engine.Util;

namespace Engine.Entities
{
	public delegate void EntityDelegate(Entity entity);

	public class Entity : MessageHub, IEquatable<Entity>
	{
		public int Id { get; protected set; }

		protected bool _disposed;

		#region constructors

		public Entity()
			: base()
		{
		}

		#endregion

		public event EntityDelegate EntityDestroyed;

		public void Reset(int id)
		{
			Id = id;
			_disposed = false;

			Components.Clear();
		}

		#region Event registry

		private void RaiseEntityDestroyed()
		{
			// TODO: reconsider using the event for the entity container
			EntityDestroyed?.Invoke(this);
			OnNext(new EntityDestroyedMessage(MessageScope.External, this));
			EntityDestroyed = null;
		}

		public override void Dispose()
		{
			_disposed = true;
			RaiseEntityDestroyed();
			base.Dispose();
		}

		~Entity()
		{
			Dispose();
		}

		#endregion


		protected EntityRegistry EntityRegistry { get; set; }


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
