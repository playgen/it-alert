using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Components.Enhacements;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public class EnhancementSystem : IInitializingSystem
	{
		private readonly List<IEnhancementSystemExtension> _enhancementSystemExtensions;

		//private readonly ComponentMatcherGroup<ISubsystemEnhancement> _enhancementMatcherGroup;

		#region constructor

		public EnhancementSystem([InjectOptional] List<IEnhancementSystemExtension> enhancementSystemExtensions) // TODO: remove zenject dependency when implicit optional collection paramters is implemented
		{
			_enhancementSystemExtensions = enhancementSystemExtensions;
			//_enhancementMatcherGroup = matcherProvider.CreateMatcherGroup<ISubsystemEnhancement>();
			//_enhancementMatcherGroup.MatchingEntityAdded += EnhancementMatcherGroupOnMatchingEntityAdded;
		}

		//private void EnhancementMatcherGroupOnMatchingEntityAdded(Entity entity)
		//{

		//}

		#endregion

		#region IInitializingSystem

		public void Initialize()
		{
		}

		#endregion

		private void ExecuteExtensionAction(Action<IEnhancementSystemExtension> action)
		{
			foreach (var extension in _enhancementSystemExtensions)
			{
				action(extension);
			}
		}
	}
}
