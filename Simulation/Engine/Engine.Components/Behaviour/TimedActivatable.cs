using System;
using Engine.Entities;
using Engine.Serialization;

namespace Engine.Components.Behaviour
{
	public abstract class TimedActivatable : Activatable, ITimedActivatableBehaviour
	{

		protected ActivationTimerUnits _timerUnits;

		private long _warmUp;
		private long _coolDown;
		private bool _canCancel;

		private long _activationRequested = -1;
		private long _deactivationRequested = -1;

		[SyncState(StateLevel.Differential)]
		public int CurrentTick { get; protected set; }

		public override bool CanActivate => IsActive == false && _activationRequested == -1 && _deactivationRequested == -1;

		public override bool CanDeactivate => IsActive && _deactivationRequested == -1;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="activationMode"></param>
		/// <param name="timerUnits"></param>
		/// <param name="warmUp">this is the number of unit before the item is activated</param>
		/// <param name="coolDown">this is the </param>
		/// <param name="canCancel"></param>
		protected TimedActivatable(ActivationMode activationMode, ActivationTimerUnits timerUnits, int warmUp, int coolDown, bool canCancel) 
			: base(activationMode)
		{
			_timerUnits = timerUnits;
			_warmUp = warmUp;
			_coolDown = coolDown;

			if (_timerUnits == ActivationTimerUnits.Milliseconds)
			{
				// we use the DateTime.Ticks property which is not measured in milliseconds
				_warmUp *= 10000;
				_coolDown *= 10000;
			}
			_canCancel = canCancel;
		}

		public override void Activate()
		{
			if (IsActive == false)
			{
				switch (_timerUnits)
				{
					case ActivationTimerUnits.Ticks:
						_activationRequested = CurrentTick;
						break;
					case ActivationTimerUnits.Milliseconds:
						_activationRequested = DateTime.Now.Ticks;
						break;
					default:
						throw new NotImplementedException("Current timer method not supported");
				}

				OnActivationRequested();
			}
		}

		public override void Deactivate()
		{
			base.Deactivate();
		}

		public virtual void OnActivationRequested()
		{
			
		}

		public virtual void OnDeactivationRequested()
		{
		}

		public override void OnDeactivating()
		{
			base.OnDeactivating();
		}

		protected virtual void OnTick()
		{
			if (IsActive == false && _activationRequested >= 0)
			{
				switch (_timerUnits)
				{
					case ActivationTimerUnits.Ticks:
						if (++_activationRequested >= _warmUp)
						{
							base.Activate();
							_activationRequested = -1;
						}
						break;
					case ActivationTimerUnits.Milliseconds:
						if (DateTime.Now.Ticks >= _activationRequested + _warmUp)
						{
							base.Activate();
							_activationRequested = -1;
						}
						break;
					default:
						throw new NotImplementedException("Current timer method not supported");
				}
			}
			else if (IsActive && _deactivationRequested >= 0)
			{
				switch (_timerUnits)
				{
					case ActivationTimerUnits.Ticks:
						if (++_deactivationRequested >= _coolDown)
						{
							_deactivationRequested = -1;
						}
						break;
					case ActivationTimerUnits.Milliseconds:
						if (DateTime.Now.Ticks >= _deactivationRequested + _coolDown)
						{
							_deactivationRequested = -1;
						}
						break;
					default:
						throw new NotImplementedException("Current timer method not supported");
				}

			}
		}

		public void Tick(int currentTick)
		{
			if (CurrentTick < currentTick)
			{
				CurrentTick = currentTick;
				OnTick();
			}
		}
	}
}
