﻿using System;
using Engine.Messaging;
using Engine.Serialization;
using Engine.Util;

namespace Engine.Entities
{
	public class Entity : MessageHub, IEntity, IEquatable<IEntity>
	{

		#region constructors
		public Entity(IEntityRegistry entityRegistry)
			: base()
		{
			NotNullHelper.ArgumentNotNull(entityRegistry, nameof(entityRegistry));
			EntityRegistry = entityRegistry;
			Id = entityRegistry.EntitySeed;
		}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		private Entity()
		{
			
		}

		#endregion

		public event EventHandler EntityDestroyed;

		#region Event registry

		private void RaiseEntityDestroyed(object entity)
		{
			// TODO: reconsider using the event for the entity container
			EntityDestroyed?.Invoke(entity, EventArgs.Empty);
			OnNext(new EntityDestroyedMessage(MessageScope.External, this));
			EntityDestroyed = null;
		}

		private bool _disposed;

		public override void Dispose()
		{
			if (_disposed == false)
			{
				_disposed = true;
				RaiseEntityDestroyed(this);
				base.Dispose();
			}
		}

		~Entity()
		{
			Dispose();
		}

		#endregion

		[SyncState(StateLevel.Differential)]
		public int Id { get; protected set; }


		protected IEntityRegistry EntityRegistry { get; set; }


		public bool Equals(IEntity other)
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