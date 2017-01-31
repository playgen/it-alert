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
using PlayGen.ITAlert.Simulation.Components.Items.Flags;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class ScannerBehaviour : IItemActivationExtension
	{
		// TODO: this all feels a bit clunky!
		public ComponentMatcher Matcher => _scannerMatcherGroup;

		private readonly IEntityRegistry _entityRegistry;

		private readonly ComponentMatcherGroup<Scanner, CurrentLocation> _scannerMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome> _malwareMatcherGroup;

		public ScannerBehaviour(IEntityRegistry entityRegistry, IComponentRegistry componentRegistry)
		{
			_entityRegistry = entityRegistry;
			_scannerMatcherGroup = componentRegistry.CreateMatcherGroup<Scanner, CurrentLocation>();
			_malwareMatcherGroup = componentRegistry.CreateMatcherGroup<MalwareGenome>();
		}

		public void OnActivating(Entity item, Activation activation)
		{
			// do nothing
		}

		public void OnActive(Entity item, Activation activation)
		{
			// do nothing
		}

		public void OnDeactivating(Entity item, Activation activation)
		{
			if (Matcher.IsMatch(item))
			{
				CurrentLocation currentLocation;
				Entity currentLocationEntity;
				Visitors currentLocationVisitors;
				if (item.TryGetComponent(out currentLocation)
					&& _entityRegistry.TryGetEntityById(currentLocation.Value, out currentLocationEntity)
					&& currentLocationEntity.TryGetComponent(out currentLocationVisitors))
				{
					// join the current locations list of visitors with all malware entities
					foreach (var malwareVisitor in currentLocationVisitors.Values
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
}
