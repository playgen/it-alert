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
	public class EnhancementSystem : Engine.Systems.System, IInitializingSystem
	{
		private readonly List<IEnhancementSystemExtension> _enhancementSystemExtensions;

		private readonly ComponentMatcherGroup<ISubsystemEnhancement> _enhancementMatcherGroup;

		#region constructor

		public EnhancementSystem(IComponentRegistry componentRegistry, 
			IEntityRegistry entityRegistry,
			// TODO: remove zenject dependency when implicit optional collection paramters is implemented
			[InjectOptional] List<IEnhancementSystemExtension> enhancementSystemExtensions)
			: base(componentRegistry, entityRegistry)
		{
			_enhancementSystemExtensions = enhancementSystemExtensions;
			_enhancementMatcherGroup = componentRegistry.CreateMatcherGroup<ISubsystemEnhancement>();
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
