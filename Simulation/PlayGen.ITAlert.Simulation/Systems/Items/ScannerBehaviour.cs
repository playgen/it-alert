using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class ScannerBehaviour : IItemActivationExtension
	{
		// TODO: this all feels a bit clunky!
		public int[] ItemIds => _scannerMatcherGroup.MatchingEntityKeys;

		private readonly IEntityRegistry _entityRegistry;

		private readonly ComponentMatcherGroup<Scanner, CurrentLocation> _scannerMatcherGroup;
		private readonly ComponentMatcherGroup<Visitors> _visitorsMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome> _malwareMatcherGroup;
		
		public ScannerBehaviour(IEntityRegistry entityRegistry, IMatcherProvider matcherProvider)
		{
			_entityRegistry = entityRegistry;
			_scannerMatcherGroup = matcherProvider.CreateMatcherGroup<Scanner, CurrentLocation>();
			_visitorsMatcherGroup = matcherProvider.CreateMatcherGroup<Visitors>();
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwareGenome>();
		}

		public void OnActivating(int itemId, Activation activation)
		{
			// do nothing
		}

		public void OnActive(int itemId, Activation activation)
		{
			// do nothing
		}

		public void OnDeactivating(int itemId, Activation activation)
		{
			ComponentEntityTuple<Scanner, CurrentLocation> itemTuple;
			ComponentEntityTuple<Visitors> locationTuple;
			if (_scannerMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple)
				&& _visitorsMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value, out locationTuple))
			{
				// join the current locations list of visitors with all malware entities
				foreach (var malwareVisitor in locationTuple.Component1.Values
					.Join(_malwareMatcherGroup.MatchingEntities,
						k => k,
						k => k.Entity.Id,
						(o, i) => i))
				{
					// add the visible gene
					malwareVisitor.Component1.Values.Add(SimulationConstants.MalwareVisibilityGene);
				}
			}
		}
	}
}
