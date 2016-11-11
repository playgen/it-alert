using Engine.Core.Components;
using Engine.Core.Entities;

namespace Engine.Components.Behaviour
{
	public abstract class ActivatableBehaviour : Component, IActivatableBehaviour
	{
		public enum ActivationTimerUnits
		{
			Ticks,  // we're only going to use this implementation for now
			Milliseconds,
			Signals,
		}


		public enum ActivationMode
		{
			OneShot,
			Timed,
		}

		private readonly ActivationMode _activationMode;

		private bool _isActive;

		public bool IsActive => _isActive;

		public abstract bool CanActivate { get; }

		public abstract bool CanDeactivate { get; }

		#region constructor

		protected ActivatableBehaviour(IEntity entity, ActivationMode activationMode) 
			: base(entity)
		{
			_activationMode = activationMode;
		}

		#endregion
		
		public virtual void OnActivating()
		{
		}

		public virtual void OnActive()
		{
		}

		public virtual void OnDeactivating()
		{
		}

		public virtual void Activate()
		{
			OnActivating();
			_isActive = true;
		}

		public virtual void Deactivate()
		{
			OnDeactivating();
			_isActive = false;
		}
	}
}
