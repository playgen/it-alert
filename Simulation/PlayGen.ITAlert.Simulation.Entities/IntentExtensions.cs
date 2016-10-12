using System;
using PlayGen.ITAlert.Simulation.Contracts.Intents;
using PlayGen.ITAlert.Simulation.Entities.Visitors.Actors.Intents;

namespace PlayGen.ITAlert.Simulation.Entities
{
	public static class IntentExtensions
	{
		public static IntentState ToIntentState(this Intent intent)
		{
			var moveIntent = intent as MoveIntent;
			if (moveIntent != null)
			{
				return new IntentState()
				{
					Action = IntentState.IntentAction.Move,
					ActionParameter = moveIntent.Destination.Id,
				};
			}
			var pickupIntent = intent as PickUpItemIntent;
			if (pickupIntent != null)
			{
				return new IntentState()
				{
					Action = IntentState.IntentAction.PickupItemType,
					ActionParameter = (int)pickupIntent.ItemType,
				};
			}
			var infectIntent = intent as InfectSystemIntent;
			if (infectIntent != null)
			{
				return new IntentState()
				{
					Action = IntentState.IntentAction.Infect,
				};
			}
			throw new Exception($"Intent not serializable {typeof(Intent)}");
		}
	}
}
