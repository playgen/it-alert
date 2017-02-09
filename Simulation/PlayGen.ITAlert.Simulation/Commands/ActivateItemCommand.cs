using Engine.Commands;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class ActivateItemCommand : ICommand
	{
		public int PlayerId { get; set; }

		public int ItemId { get; set; }
	}

	public class ActivateItemCommandHandler : CommandHandler<ActivateItemCommand>
	{
		private readonly ComponentMatcherGroup<Item, Activation, CurrentLocation, Owner> _activationMatcherGroup;
		// TODO: match subsystems on presence of activationcontainer once theyr are refactored into independent entities
		private ComponentMatcherGroup<Subsystem> _subsystemMatcherGroup; 

		public ActivateItemCommandHandler(IMatcherProvider matcherProvider)
		{
			_activationMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Activation, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem>();
		}

		protected override bool TryProcessCommand(ActivateItemCommand command)
		{
			ComponentEntityTuple<Item, Activation, CurrentLocation, Owner> itemTuple;
			ComponentEntityTuple<Subsystem> subsystemTuple;
			if (_activationMatcherGroup.TryGetMatchingEntity(command.ItemId, out itemTuple)
				&& itemTuple.Component2.ActivationState == ActivationState.NotActive
				&& (itemTuple.Component4.Value == null || itemTuple.Component4.Value == command.PlayerId)
				&& itemTuple.Component3.Value.HasValue
				&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component3.Value.Value, out subsystemTuple))
			{
				itemTuple.Component2.Activate();
				itemTuple.Component4.Value = command.PlayerId;
				return true;
			}
			return false;
		}
	}
}
