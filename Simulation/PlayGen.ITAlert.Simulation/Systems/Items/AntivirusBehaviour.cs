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
	public class AntivirusBehaviour : IItemActivationExtension
	{
		private readonly ComponentMatcherGroup<Antivirus, CurrentLocation, Owner> _antivirusMatcherGroup;
		private readonly ComponentMatcherGroup<Visitors> _visitorsMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome> _malwareMatcherGroup;
		
		public AntivirusBehaviour(IMatcherProvider matcherProvider)
		{
			_antivirusMatcherGroup = matcherProvider.CreateMatcherGroup<Antivirus, CurrentLocation, Owner>();
			_visitorsMatcherGroup = matcherProvider.CreateMatcherGroup<Visitors>();
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
			ComponentEntityTuple<Antivirus, CurrentLocation, Owner> itemTuple;
			if (_antivirusMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple))
			{
				ComponentEntityTuple<Visitors> locationTuple;
				if (itemTuple.Component2.Value.HasValue
					&&_visitorsMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out locationTuple))
				{
					// join the current locations list of visitors with all malware entities
					foreach (var malwareVisitor in locationTuple.Component1.Values
						.Join(_malwareMatcherGroup.MatchingEntities,
							k => k,
							k => k.Entity.Id,
							(o, i) => i))
					{
						if ((malwareVisitor.Component1.Value & itemTuple.Component1.TargetGenome) == malwareVisitor.Component1.Value)
						{
							malwareVisitor.Entity.Dispose();
						}
					}
				}
				itemTuple.Component3.Value = null;
			}
		}
	}
}
