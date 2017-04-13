using System.Linq;
using Engine.Components;
using Engine.Systems.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Modules.Malware.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems.Activation
{
	public class AntivirusBehaviour : IActivationExtension
	{
		private readonly ComponentMatcherGroup<Components.Antivirus, CurrentLocation, Owner> _antivirusMatcherGroup;
		private readonly ComponentMatcherGroup<Visitors> _visitorsMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome> _malwareMatcherGroup;
		
		public AntivirusBehaviour(IMatcherProvider matcherProvider)
		{
			_antivirusMatcherGroup = matcherProvider.CreateMatcherGroup<Components.Antivirus, CurrentLocation, Owner>();
			_visitorsMatcherGroup = matcherProvider.CreateMatcherGroup<Visitors>();
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
					&&_visitorsMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out var locationTuple))
				{
					// join the current locations list of visitors with all malware entities
					foreach (var malwareVisitor in locationTuple.Component1.Values
						.Join(_malwareMatcherGroup.MatchingEntities,
							k => k,
							k => k.Entity.Id,
							(o, i) => i).ToArray())
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
