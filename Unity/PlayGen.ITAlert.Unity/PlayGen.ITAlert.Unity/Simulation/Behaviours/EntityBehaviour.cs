using System;
using Engine.Entities;
using GameWork.Core.Logging.Loggers;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	/// <summary>
	/// Base class for all behaviour scripts attached to simulator entities
	/// </summary>
	public abstract class EntityBehaviour : MonoBehaviour, IEntityBehaviour
	{
        public event Action StateUpdated;

		/// <summary>
		/// Has initialization been performed yet, prevents unity update methods from proceeding
		/// </summary>
		protected bool Initialized { get; private set; }

		/// <summary>
		/// state of this entity
		/// </summary>
		public Entity Entity { get; private set; }

		protected Director Director { get; private set; }

		private TutorialHighlight _tutorialHighlight;

		/// <summary>
		/// Id of this entity
		/// </summary>
		public int Id => Entity.Id;

		public string Name { get; set; }


		[SerializeField]
		private GameObject _highlight;

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

		private void UpdateHighlight()
		{
			if (_tutorialHighlight != null && _highlight.activeSelf != _tutorialHighlight.Enabled)
			{
				_highlight.SetActive(_tutorialHighlight.Enabled);
			}
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
		protected virtual void OnStateUpdated()
		{
			UpdateHighlight();
            StateUpdated?.Invoke();
        }

        /// <summary>
        /// Initialize the object from simulation state
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="director"></param>
        public void Initialize(Entity entity, Director director)
		{
			Entity = entity;
			Director = director;
			Entity.TryGetComponent(out _tutorialHighlight);
			OnInitialize();
			Initialized = true;

			LogProxy.Info($"EntityBehviour Initialize: GameObject {gameObject.name} Entity {Entity?.Id.ToString() ?? "null"} Director {Director?.InstanceId ?? "null"}");

		}

		//public void Uninitialize()
		//{
		//	Initialized = false;
		//	OnUninitialize();
		//	Entity = null;
		//}

		/// <summary>
		/// actions to perform on initialization
		/// </summary>
		protected abstract void OnInitialize();

		//protected virtual void OnUninitialize()
		//{
			
		//}

		public virtual void ResetEntity()
		{
			
		}

		public virtual void UpdateScale(Vector3 scale)
		{
			
		}

		public void OnDestroy()
		{
			LogProxy.Info($"EntityBehviour Destroy: GameObject {gameObject.name} Entity {Entity?.Id.ToString() ?? "null"} Director {Director?.InstanceId ?? "null"}");
		}

		#endregion
	}
}