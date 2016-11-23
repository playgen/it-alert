using System;
using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Systems;

namespace PlayGen.ITAlert.Simulation.VisitorsProperty.Items
{
	/// <summary>
	/// Base class for items that can modify the state of subsystems
	/// </summary>
	public abstract class Item : Visitor, IItem
	{
		[SyncState(StateLevel.Setup)]
		public ItemType ItemType { get; protected set; }

		/// <summary>
		/// Indicate whether the item should be destroyed when deactivated
		/// </summary>
		[SyncState(StateLevel.Setup)]
		public bool IsConsumable { get; protected set; }

		#region activation timing

		/// <summary>
		/// Indicate whether the item is currently performing it's action
		/// </summary>
		public bool IsActive => ActiveTicksRemaining == -1 || ActiveTicksRemaining > 0;

		/// <summary>
		/// number of ticks the item will perform action for when activated
		/// </summary>
		[SyncState(StateLevel.Differential)]
		protected int ActiveDuration { get; set; }

		/// <summary>
		/// counter before automatic deactivation
		/// </summary>
		[SyncState(StateLevel.Differential)]
		protected int ActiveTicksRemaining { get; set; }

		/// <summary>
		/// Returns the actor owning this item, or null if unowned
		/// </summary>
		[SyncState(StateLevel.Differential)]
		public IActor Owner { get; protected set; }

		public bool HasOwner => Owner != null;

		#endregion


		protected System CurrentSystem { get; set; }

		public bool IsOnSystem => CurrentSystem != null;


		#region constructors

		/// <summary>
		/// 
		/// </summary>
		/// <param name="simulation"></param>
		/// <param name="itemType">The type of this item</param>
		/// <param name="isConsumable">should item be destroyed when deactivated</param>
		/// <param name="activeDuration">number of ticks to perform action or when activated or -1 for perptuity</param>
		protected Item(ISimulation simulation, ItemType itemType, bool isConsumable, int activeDuration)// int cooldown)
			: base(simulation, EntityType.Item)
		{
			IsConsumable = isConsumable;
			ActiveDuration = activeDuration;
			ItemType = itemType;
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected Item()
		{
			
		}

		#endregion

		#region State

		public override ItemState GenerateState()
		{
			return new ItemState(Id, ItemType)
			{
				Active = IsActive,
				ActiveDuration = ActiveDuration,
				ActiveTicksRemaining = ActiveTicksRemaining,
				Owner = Owner?.Id,
				CurrentNode = CurrentNode?.Id,
				//TODO: reimplement in subclass when they can be activated elsewhere
				CanActivate = CurrentNode is System,
			};
		}

		#endregion

		#region Tick

		protected override void OnTick()
		{
			if (IsActive)
			{
				OnItemActive();
				ActiveTicksRemaining--;
				if (ActiveTicksRemaining == 0)
				{
					Deactivate();
				}
			}
		}

		#endregion

		#region Activation

		protected virtual void OnItemActivating()
		{
			// nothing to do here	
		}

		protected virtual void OnItemActive()
		{
			// nothing to do here	
		}

		protected virtual void OnItemDeactivating()
		{
			if (IsConsumable)
			{
				Dispose();
			}
		}

		public virtual void Activate()
		{
			if (!IsActive)
			{
				ActiveTicksRemaining = ActiveDuration;
				OnItemActivating();
			}
		}

		public virtual void Deactivate()
		{
			OnItemDeactivating();
			ActiveTicksRemaining = ActiveDuration == -1 ? ActiveDuration : 0;
		}

		public virtual bool CanBeActivatedBy(IActor actor)
		{
			return IsOwnedBy(actor) && IsActive == false;
		}

		public void SetOwnership(IActor actor)
		{
			Owner = actor;
		}

		public bool IsOwnedBy(IActor actor)
		{
			return actor.Equals(Owner);
		}



		#endregion

		#region Movement

		public override void OnEnterNode(INode current)
		{
			CurrentSystem = current as System;
			
			base.OnEnterNode(current);
		}

		public override void OnExitNode(INode current)
		{
			CurrentSystem = null;

			base.OnExitNode(current);
		}

		public void PickUp()
		{
			CurrentSystem?.GetItem(this);
		}

		
		public bool CanBeDropped()
		{
			var currentSystem = CurrentNode as System ?? Owner?.CurrentNode as System;
			return currentSystem != null 
				&& currentSystem.CanAddItem() 
				&& currentSystem.HasItem(this) == false;
		}

		public void Disown()
		{
			SetOwnership(null);
		}

		public bool Drop()
		{
			var currentSystem = CurrentNode as System ?? Owner?.CurrentNode as System;
			if (currentSystem != null)
			{
				if (currentSystem.HasItem(this) == false && CanBeDropped())
				{
					currentSystem.TryAddItem(this);
				}
				SetOwnership(null);
				return true;
			}
			return false;
		}

		#endregion
	}
}
	