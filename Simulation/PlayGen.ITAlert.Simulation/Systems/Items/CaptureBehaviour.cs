using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Systems.Enhancements;
using PlayGen.ITAlert.Simulation.Systems.Extensions;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class CaptureBehaviour : IItemActivationExtension
	{
		public const string AnalysisOutputArchetypeName = "Antivirus";
		
		private readonly ComponentMatcherGroup<Capture, CurrentLocation, Owner> _captureMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, Visitors> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<MalwareGenome, MalwareVisibility> _malwareMatcherGroup;

		public CaptureBehaviour(IMatcherProvider matcherProvider)
		{
			_captureMatcherGroup = matcherProvider.CreateMatcherGroup<Capture, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, Visitors>();
			_malwareMatcherGroup = matcherProvider.CreateMatcherGroup<MalwareGenome, MalwareVisibility>();
		}

		public void OnNotActive(int itemId, Activation activation)
		{

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
			ComponentEntityTuple<Capture, CurrentLocation, Owner> itemTuple;
			if (_captureMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple))
			{
				ComponentEntityTuple<Subsystem, Visitors> locationTuple;
				if (itemTuple.Component2.Value.HasValue
					&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out locationTuple))
				{
					// join the current locations list of visitors with all malware entities
					var malwareVisitor = locationTuple.Component2.Values
						.Join(_malwareMatcherGroup.MatchingEntities,
							k => k,
							k => k.Entity.Id,
							(o, i) => i)
						.FirstOrDefault();

					// TODO: probably need a better way of choosing the malware than selecting first, but this will do for now
					if (malwareVisitor != null
						&& itemTuple.Component3.Value.HasValue
						&& malwareVisitor.Component2.VisibleTo.Contains(itemTuple.Component3.Value.Value))
					{
						itemTuple.Component1.CapturedGenome = malwareVisitor.Component1.Value;
					}
				}
			}
		}
	}
}
