using System;
using Engine.Entities;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
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

		protected virtual void OnFixedUpdate()
		{
			
		}

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

		protected virtual void OnUpdate()
		{
			
		}

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
			Entity = entity;
			OnInitialize();
			Initialized = true;
		}

		public void Uninitialize()
		{
			Initialized = false;
			OnUninitialize();
			Entity = null;
		}

		/// <summary>
		/// actions to perform on initialization
		/// </summary>
		protected abstract void OnInitialize();

		protected virtual void OnUninitialize()
		{
			
		}

		#endregion
	}
}