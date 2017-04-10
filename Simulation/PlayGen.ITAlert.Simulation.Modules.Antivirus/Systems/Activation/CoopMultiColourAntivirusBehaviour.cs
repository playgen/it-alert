using System.Linq;
using Engine.Components;
using Engine.Systems.Activation;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems.Activation
{
	public class CoopMultiColourAntivirusBehaviour : IActivationExtension
	{
		private const bool AllowSamePlayerActivation = false;

		private readonly ComponentMatcherGroup<Components.Antivirus, CurrentLocation, Owner, Engine.Systems.Activation.Components.Activation> _antivirusMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage, Visitors> _locationMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome> _malwareMatcherGroup;
		
		public CoopMultiColourAntivirusBehaviour(IMatcherProvider matcherProvider)
		{
			_antivirusMatcherGroup = matcherProvider.CreateMatcherGroup<Components.Antivirus, CurrentLocation, Owner, Engine.Systems.Activation.Components.Activation>();
			_locationMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage, Visitors>();
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwareGenome>();
		}

		public void OnNotActive(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			
		}

		public void OnActivating(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{

		}

		public void OnActive(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			// do nothing
		}

		public void OnDeactivating(int itemId, Engine.Systems.Activation.Components.Activation activation)
		{
			if (_antivirusMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple))
			{
				if (itemTuple.Component2.Value.HasValue
					&& _locationMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out var locationTuple))
				{
					var combinedGenome = itemTuple.Component1.TargetGenome | locationTuple.Component2.Items.Where(ic => ic?.Item != null && ic.Item.Value != itemId)
							.Join(_antivirusMatcherGroup.MatchingEntities.Where(av => av.Component4.ActivationState == ActivationState.Active && (AllowSamePlayerActivation || av.Component3.Value != itemTuple.Component3.Value)),
								k => k.Item.Value,
								k => k.Entity.Id,
								(o, i) => i)
							.Aggregate(0, (g, av) => g |= av.Component1.TargetGenome, g => g);
					if (combinedGenome != 0 && combinedGenome != itemTuple.Component1.TargetGenome)
					{
						// join the current locations list of visitors with all malware entities
						foreach (var malwareVisitor in locationTuple.Component3.Values
							.Join(_malwareMatcherGroup.MatchingEntities,
								k => k,
								k => k.Entity.Id,
								(o, i) => i))
						{
							if ((malwareVisitor.Component1.Value & combinedGenome) == malwareVisitor.Component1.Value)
							{
								malwareVisitor.Entity.Dispose();
							}
						}
						
					}
				}
				itemTuple.Component3.Value = null;
			}
		}
	}
}
