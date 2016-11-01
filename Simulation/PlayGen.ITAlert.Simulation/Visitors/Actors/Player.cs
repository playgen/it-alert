using System;
using System.Collections.Generic;
using Engine.Core.Components;
using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Systems;
using PlayGen.ITAlert.Simulation.Visitors.Actors.Intents;

namespace PlayGen.ITAlert.Simulation.Visitors.Actors
{
	public class Player : Actor<PlayerState>
	{
		[SyncState(StateLevel.Setup)]
		public string Name { get; private set; }

		[SyncState(StateLevel.Setup)]
		public string Colour { get; private set;  }

		/// <summary>
		/// Currently held items
		/// </summary>
		[SyncState(StateLevel.Differential)]
		public IItem Item { get; private set; }

		public bool HasItem => Item != null;

		public Systems.System CurrentSystem { get; private set; }

		public bool IsOnSystem => CurrentSystem != null;

		#region constructors

		public Player(ISimulation simulation, 
			IComponentContainer componentContainer, 
			string name, 
			string colour, 
			int movementSpeed = SimulationConstants.PlayerSpeed
		) : base(simulation, 
				componentContainer, 
				EntityType.Player, 
				movementSpeed)
		{
			Name = name;
			Colour = colour;
		}

		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		public Player()
		{
			
		}

		#endregion

		#region State

		public override PlayerState GenerateState()
		{
			return new PlayerState(Id)
			{
				Name = Name,
				Colour = Colour,

				InventoryItem = Item?.Id,
			};
		}

		public override void OnDeserialized()
		{
			CurrentSystem = CurrentNode as System;
			if (Item != null)
			{
				Item.EntityDestroyed += Item_EntityDestroyed;
			}
			base.OnDeserialized();
		}

		#endregion

		#region Movement 

		public override void OnEnterNode(INode current)
		{
			CurrentSystem = current as System;
			base.OnEnterNode(current);
		}

		#endregion

		#region Commands

		public void PickUpItem(ItemType itemType, int itemLocation, System subsystem)
		{
			var intents = new List<Intent>() {new PickUpItemIntent(subsystem, itemType, itemLocation)};

			//if (IsOnSystem && HasItem && CurrentSystem.HasItem(Item))
			//{
			//	intents.Add(new DisownItemIntent());
			//}

			SetIntents(intents);
		}

		public void DisownItem()
		{
			SetIntents(new Intent[] { new DisownItemIntent() });
		}

		//public void DropItem()
		//{
		//	SetIntents(new Intent[] { new DisownItemIntent() });
		//}


		public void SetItem(IItem item)
		{
			if (Item != null && Item != item)
			{
				Item.EntityDestroyed -= Item_EntityDestroyed;
				Item.SetOwnership(null);
			}
			if (item != null)
			{
				item.EntityDestroyed += Item_EntityDestroyed;
				item.SetOwnership(this);
			}
			Item = item;
		}

		public void SetDestination(INode destination)
		{
			if (destination.Equals(CurrentNode))
			{
				Intents.Clear();
			}
			else
			{
				SetIntents(new Intent[] { new MoveIntent(destination) });
			}
		}

		#endregion

		#region Tick

		protected override void OnTick()
		{
			Intent currentIntent;
			if (TryGetIntent(out currentIntent))
			{
				if (currentIntent is DisownItemIntent)
				{
					DisownItem(true);
				}
				
				var pickupItemIntent = currentIntent as PickUpItemIntent;
				if (pickupItemIntent != null)
				{
					PickupItem(pickupItemIntent);
				}
				else
				{
					var moveIntent = currentIntent as MoveIntent;
					if (moveIntent != null)
					{
						if (CurrentNode == moveIntent.Destination)
						{
							Intents.Pop();
							if (HasItem && Item.CanBeDropped())
							{
								CurrentSystem.TryAddItem(Item);
							}
						}
						else if (Item?.IsOnSystem ?? false)
						{
							CurrentSystem.GetItem(Item);
						}
					}
				}
			}
		}
		
		private void DisownItem(bool pop)
		{
			if (HasItem == false && pop)
			{
				Intents.Pop();
			}
			else if (HasItem)
			{
				SetItem(null);
			}
		}

		private void DropItem(IItem item)
		{
			Item.Drop();
			SetItem(null);
		}

		private void PickupItem(PickUpItemIntent pickupItemIntent)
		{
			if (IsOnSystem && CurrentSystem.Equals(pickupItemIntent.Destination))
			{
				IItem item;
				if (CurrentSystem.TryGetItem(pickupItemIntent.ItemType, this, pickupItemIntent.ItemLocation, out item)
					&& (Item?.Equals(item) ?? false) == false)
				{
					if (HasItem)
					{
						if (CurrentSystem.HasItem(Item) == false && CurrentSystem.CanAddItem() == false)
						{
							CurrentSystem.GetItem(item);
						}
						Item?.Drop();
						SetItem(null);
					}
					SetItem(item);
				}
				Intents.Pop();
			}
		}

		private void Item_EntityDestroyed(object sender, EventArgs e)
		{
			var entity = (IITAlertEntity) sender;
			if (entity.Equals(Item))
			{
				Item = null;
			}
			entity.EntityDestroyed -= Item_EntityDestroyed;
		}

		#endregion


	}
}
