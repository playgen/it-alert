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
	public class ScannerBehaviour : IItemActivationExtension
	{
		private readonly ComponentMatcherGroup<Scanner, CurrentLocation, Owner> _scannerMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, Visitors> _visitorsMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome, MalwareVisibility> _malwareMatcherGroup;
		
		public ScannerBehaviour(IMatcherProvider matcherProvider)
		{
			_scannerMatcherGroup = matcherProvider.CreateMatcherGroup<Scanner, CurrentLocation, Owner>();
			_visitorsMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, Visitors>();
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwareGenome, MalwareVisibility>();
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
			ComponentEntityTuple<Scanner, CurrentLocation, Owner> itemTuple;
			if (_scannerMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple))
			{
				ComponentEntityTuple<Subsystem, Visitors> locationTuple;
				if (itemTuple.Component2.Value.HasValue
					&&_visitorsMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out locationTuple))
				{
					// join the current locations list of visitors with all malware entities
					foreach (var malwareVisitor in locationTuple.Component2.Values
						.Join(_malwareMatcherGroup.MatchingEntities,
							k => k,
							k => k.Entity.Id,
							(o, i) => i))
					{
						// add the visible gene
						malwareVisitor.Component2.Visible = true;
					}
				}
				itemTuple.Component3.Value = null;
			}
		}
	}
}
