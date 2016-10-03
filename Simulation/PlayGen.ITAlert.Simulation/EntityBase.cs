using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Common.Serialization;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Interfaces;

namespace PlayGen.ITAlert.Simulation
{
	public abstract class EntityBase : IEntity, IEquatable<IEntity>
	{
		#region Event registry

		//private static int _seedId;

		//public event EventHandler EntityCreated;

		public event EventHandler EntityDestroyed;

		//private void RaiseEntityCreated(object entity)
		//{
		//	EntityCreated?.Invoke(entity, EventArgs.Empty);
		//}

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

		[SyncState(StateLevel.Minimal)]
		public int CurrentTick { get; protected set; }

		[SyncState(StateLevel.Minimal)]
		public int Id { get; protected set; }

		[SyncState(StateLevel.Setup)]
		public EntityType EntityType { get; protected set; }

		[SyncState(StateLevel.Setup)]
		protected ISimulation Simulation { get; set; }


		#region constructors

		protected EntityBase(ISimulation simulation, EntityType entityType)
		{
			// TODO: replace dirty hack for the entity factory
			Simulation = simulation;
			Id = simulation.EntitySeed;
			EntityType = entityType;
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


		public virtual void Tick(int currentTick)
		{
			if (CurrentTick < currentTick)
			{
				CurrentTick = currentTick;
				OnTick();
			}
		}
		protected abstract void OnTick();

		public virtual void OnDeserialized()
		{
		}
	}
}
