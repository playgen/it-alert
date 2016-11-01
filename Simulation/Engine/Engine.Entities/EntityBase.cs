using System;
using Engine.Components;
using Engine.Core.Entities;
using Engine.Core.Serialization;
using Engine.Core.Util;
using Engine.Entities.Messages;

namespace Engine.Entities
{
	public abstract class EntityBase<TGameState> : ComponentContainer, IEntity, IEquatable<IEntity>
		where TGameState : EntityState
	{
		#region Event registry

		private void RaiseEntityDestroyed(object entity)
		{
			//EntityDestroyed?.Invoke(entity, EventArgs.Empty);
			MessageHub.OnNext(new EntityDestroyedMessage(this));
		}

		private bool _disposed;

		public virtual void Dispose()
		{
			if (_disposed == false)
			{
				_disposed = true;
				RaiseEntityDestroyed(this);
				base.Dispose();
			}
		}

		~EntityBase()
		{
			Dispose();
		}

		#endregion

		[SyncState(StateLevel.Differential)]
		public int Id { get; protected set; }

		[SyncState(StateLevel.Setup)]
		protected IEntityRegistry EntityRegistry { get; set; }

		#region constructors

		protected EntityBase(IEntityRegistry entityRegistry)
		{
			NotNullHelper.ArgumentNotNull(entityRegistry, nameof(entityRegistry));
			EntityRegistry = entityRegistry;
			Id = entityRegistry.EntitySeed;
		}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		// TODO: cehck if this needs a different signature
		protected EntityBase()
		{
			
		}

		#endregion

		public abstract TGameState GetState();


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
