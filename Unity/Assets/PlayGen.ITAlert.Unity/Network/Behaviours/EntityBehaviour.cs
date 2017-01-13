using System;
using PlayGen.ITAlert.Simulation;
using PlayGen.ITAlert.Simulation.Contracts;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Network.Behaviours
{
	// ReSharper disable once CheckNamespace

	/// <summary>
	/// Base class for all behaviour scripts attached to simulator entities
	/// </summary>
	/// <typeparam name="TState">Type of the state object for this entity</typeparam>
	public abstract class EntityBehaviour<TState> : MonoBehaviour, IEntityBehaviour
		where TState : EntityState
	{
		/// <summary>
		/// Has initialization been performed yet, prevents unity update methods from proceeding
		/// </summary>
		protected bool Initialized { get; private set; }

		/// <summary>
		/// state of this entity
		/// </summary>
		protected TState EntityState { get; private set; }

		/// <summary>
		/// Id of this entity
		/// </summary>
		public int Id
		{
			get { return EntityState.Id; }
		}

		public EntityType EntityType
		{
			get { return EntityState.EntityType; }
		}


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
		/// verify the type of the state received
		/// </summary>
		/// <param name="entityState">entity state</param>
		protected void SetState(EntityState entityState)
		{
			var state = entityState as TState;
			if (state == null)
			{
				throw new InvalidCastException("EntityState is of wrong type for entity");
			}
			EntityState = state;
		}

		/// <summary>
		/// Set state from simulator
		/// </summary>
		/// <param name="state"></param>
		public void UpdateState(EntityState state)
		{
			SetState(state);
			OnUpdatedState();
		}

		/// <summary>
		/// actions to perform when state has been updated
		/// </summary>
		protected abstract void OnUpdatedState();

		/// <summary>
		/// Initialize the object from simulation state
		/// </summary>
		/// <param name="state"></param>
		public void Initialize(EntityState state)
		{
			SetState(state);
			OnInitialize();
			Initialized = true;
		}

		/// <summary>
		/// actions to perform on initialization
		/// </summary>
		protected abstract void OnInitialize();

		#endregion
	}
}