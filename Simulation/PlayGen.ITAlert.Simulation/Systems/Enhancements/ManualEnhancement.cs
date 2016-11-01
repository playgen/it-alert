using System;
using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public abstract class ManualEnhancement : SystemEnhancement, IActivatable
	{
		/// <summary>
		/// Indicate whether the item is currently performing it's action
		/// </summary>
		[SyncState(StateLevel.Differential)]
		public bool IsActive { get; private set; }

		#region activation timing

		/// <summary>
		/// number of ticks the item will perform action for when activated
		/// </summary>
		[SyncState(StateLevel.Differential)]
		protected int ActiveDuration { get; }

		/// <summary>
		/// counter before automatic deactivation
		/// </summary>
		[SyncState(StateLevel.Differential)]
		protected int ActiveTicksRemaining;

		#endregion

		#region constructors

		protected ManualEnhancement(ISimulation simulation, EnhancementType enhancementType, System subsystem, int activeDuration) 
			: base(simulation, enhancementType, subsystem)
		{
			ActiveDuration = activeDuration;
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected ManualEnhancement()
		{
			
		}

		#endregion

		public virtual void Activate()
		{
			if (!IsActive)
			{
				ActiveTicksRemaining = ActiveDuration;
				IsActive = true;
			}
		}

		public virtual void Deactivate()
		{
			IsActive = false;
		}

		protected override void OnTick()
		{
			var active = ActiveTicksRemaining == -1 || ActiveTicksRemaining-- > 0;

			if (active && IsActive)
			{
				OnItemActive(System);
			}
			if (ActiveTicksRemaining == 0)
			{
				Deactivate();
			}
		}

		protected abstract void OnItemActive(System subsystem);

	}
}
