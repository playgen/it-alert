using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class CoopMultiColourAntivirusBehaviour : IItemActivationExtension
	{
		private const bool AllowSamePlayerActivation = false;

		private readonly ComponentMatcherGroup<Antivirus, CurrentLocation, Owner, Activation> _antivirusMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage, Visitors> _locationMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome> _malwareMatcherGroup;
		
		public CoopMultiColourAntivirusBehaviour(IMatcherProvider matcherProvider)
		{
			_antivirusMatcherGroup = matcherProvider.CreateMatcherGroup<Antivirus, CurrentLocation, Owner, Activation>();
			_locationMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage, Visitors>();
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwareGenome>();
		}

		public void OnNotActive(int itemId, Activation activation)
		{
			
		}

		public void OnActivating(int itemId, Activation activation)
		{

		}

		public void OnActive(int itemId, Activation activation)
		{
			// do nothing
		}

		public void OnDeactivating(int itemId, Activation activation)
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
