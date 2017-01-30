using System;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Network.Behaviours
{
	// ReSharper disable once CheckNamespace

	/// <summary>
	/// Base class for all behaviour scripts attached to simulator entities
	/// </summary>
	public abstract class EntityBehaviour : MonoBehaviour, IEntityBehaviour
	{
		/// <summary>
		/// Has initialization been performed yet, prevents unity update methods from proceeding
		/// </summary>
		protected bool Initialized { get; private set; }

		/// <summary>
		/// state of this entity
		/// </summary>
		public Entity Entity { get; private set; }

		/// <summary>
		/// Id of this entity
		/// </summary>
		public int Id => Entity.Id;

		public EntityType EntityType { get; private set; }
		
		#region Unity Update

		/// <summary>
		/// Do not override, this needs the initialization check
		/// Implement update in OnFixedUpdate abstract implementation
		/// </summary>
		public void FixedUpdate()
		{
			if (Initialized)
			{
				OnFixedUpdate();
			}
		}

		protected abstract void OnFixedUpdate();

		/// <summary>
		/// Do not override, this needs the initialization check
		/// Implement update in OnUpdate abstract implementation
		/// </summary>
		public void Update()
		{
			if (Initialized)
			{
				OnUpdate();
			}
		}

		protected abstract void OnUpdate();

		#endregion

		#region State Update

		/// <summary>

		/// Set state from simulator
		/// </summary>
		public void UpdateState()
		{
			OnStateUpdated();
		}

		/// <summary>
		/// actions to perform when state has been updated
		/// </summary>
		protected abstract void OnStateUpdated();

		/// <summary>
		/// Initialize the object from simulation state
		/// </summary>
	/// <param name="entity"></param>
		public void Initialize(Entity entity)
		{
			EntityTypeProperty entityType;
			if (entity.TryGetComponent(out entityType))
			{
				EntityType = entityType.Value;
				Entity = entity;
				OnInitialize();
				Initialized = true;
			}
			else
			{
				throw new InvalidOperationException("Could read entity type component!");
			}

		}
		/// <summary>
		/// actions to perform on initialization
		/// </summary>
		protected abstract void OnInitialize();

		#endregion
	}
}