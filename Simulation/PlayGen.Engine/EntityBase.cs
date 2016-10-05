using System;

namespace PlayGen.Engine
{
	public abstract class EntityBase : IEntity, IEquatable<IEntity>
	{
		#region Event registry

		public event EventHandler EntityDestroyed;

		private void RaiseEntityDestroyed(object entity)
		{
			EntityDestroyed?.Invoke(entity, EventArgs.Empty);
		}

		private bool _disposed;

		public virtual void Dispose()
		{
			if (_disposed == false)
			{
				_disposed = true;
				RaiseEntityDestroyed(this);
			}
		}

		~EntityBase()
		{
			Dispose();
		}

		#endregion


		//[SyncState(StateLevel.Minimal)]
		public int Id { get; protected set; }

		//[SyncState(StateLevel.Setup)]
		protected IEntityRegistry EntityRegistry { get; set; }
		
		#region constructors

		protected EntityBase(IEntityRegistry entityRegistry)
		{
			// TODO: replace dirty hack for the entity factory
			EntityRegistry = entityRegistry;
			Id = entityRegistry.EntitySeed;
			//RaiseEntityCreated(this);
		}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		// TODO: cehck if this needs a different signature
		protected EntityBase()
		{
			
		}

		#endregion

		public abstract EntityState GetState();


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
