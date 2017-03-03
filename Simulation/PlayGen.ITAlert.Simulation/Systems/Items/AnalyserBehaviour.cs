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
using PlayGen.ITAlert.Simulation.Systems.Enhancements;
using PlayGen.ITAlert.Simulation.Systems.Extensions;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class AnalyserBehaviour : IItemActivationExtension
	{
		public const string AnalysisOutputArchetypeName = "Antivirus";
		
		private readonly ComponentMatcherGroup<Analyser, CurrentLocation, Owner> _analyserMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<Capture> _captureMatcherGroup;

		private readonly IEntityFactoryProvider _entityFactoryProvider;

		public AnalyserBehaviour(IMatcherProvider matcherProvider, IEntityFactoryProvider entityFactoryProvider)
		{
			_analyserMatcherGroup = matcherProvider.CreateMatcherGroup<Analyser, CurrentLocation, Owner>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_captureMatcherGroup = matcherProvider.CreateMatcherGroup<Capture>();

			_entityFactoryProvider = entityFactoryProvider;
		}

		public void OnNotActive(int itemId, Activation activation)
		{
			ComponentEntityTuple<Analyser, CurrentLocation, Owner> itemTuple;
			if (_analyserMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple)
				&& itemTuple.Component2.Value.HasValue
				&& itemTuple.Component3.Value.HasValue)
			{
				itemTuple.Component3.Value = null;
			}
		}

		public void OnActivating(int itemId, Activation activation)
		{
			ComponentEntityTuple<Analyser, CurrentLocation, Owner> itemTuple;
			if (_analyserMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple))
			{
				ComponentEntityTuple<Subsystem, ItemStorage> locationTuple;
				AnalysisTargetItemContainer analysisTargetItemContainer;
				ComponentEntityTuple<Capture> captureTuple;
				if (itemTuple.Component2.Value.HasValue
					&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out locationTuple)
					&& locationTuple.Component2.TryGetItemContainer(out analysisTargetItemContainer)
					&& analysisTargetItemContainer.Item.HasValue
					&& _captureMatcherGroup.TryGetMatchingEntity(analysisTargetItemContainer.Item.Value, out captureTuple)
					&& captureTuple.Component1.CapturedGenome != 0)
				{
				}
				else
				{
					activation.CancelActivation();
				}
			}

		}

		public void OnActive(int itemId, Activation activation)
		{
			// do nothing
		}

		public void OnDeactivating(int itemId, Activation activation)
		{
			ComponentEntityTuple<Analyser, CurrentLocation, Owner> itemTuple;
			if (_analyserMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple))
			{
				ComponentEntityTuple<Subsystem, ItemStorage> locationTuple;
				AnalysisTargetItemContainer analysisTargetItemContainer;
				AnalysisOutputItemContainer analysisOutputItemContainer;
				ComponentEntityTuple<Capture> captureTuple;
				if (itemTuple.Component2.Value.HasValue
					&& _subsystemMatcherGroup.TryGetMatchingEntity(itemTuple.Component2.Value.Value, out locationTuple)
					&& locationTuple.Component2.TryGetItemContainer(out analysisTargetItemContainer)
					&& analysisTargetItemContainer.Item.HasValue
					&& _captureMatcherGroup.TryGetMatchingEntity(analysisTargetItemContainer.Item.Value, out captureTuple)
					&& captureTuple.Component1.CapturedGenome != 0
					&& locationTuple.Component2.TryGetItemContainer(out analysisOutputItemContainer)
					&& analysisOutputItemContainer.Item.HasValue == false)
				{
					ComponentEntityTuple<CurrentLocation, Owner> antivirusEntityTuple;
					Antivirus antivirus;
					if (_entityFactoryProvider.TryCreateItem(AnalysisOutputArchetypeName, locationTuple.Entity.Id, null, out antivirusEntityTuple)
						&& antivirusEntityTuple.Entity.TryGetComponent(out antivirus))
					{
						antivirus.TargetGenome = captureTuple.Component1.CapturedGenome;
						analysisOutputItemContainer.Item = antivirusEntityTuple.Entity.Id;
					}
					else
					{
						antivirusEntityTuple.Entity.Dispose();
					}
				}
			}
		}
	}
}
