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

namespace PlayGen.ITAlert.Simulation.Systems.Enhancements
{
	public class EnhancementSystem : Engine.Systems.System, IInitializingSystem
	{
		private readonly List<IEnhancementSystemExtension> _enhancementSystemExtensions;

		private readonly ComponentMatcherGroup _enhancementMatcherGroup;

		public EnhancementSystem(ComponentRegistry componentRegistry, EntityRegistry entityRegistry, SystemRegistry systemRegistry)
			: base(componentRegistry, entityRegistry, systemRegistry)
		{
			_enhancementSystemExtensions = ModuleLoader.InstantiateTypesImplementing<IEnhancementSystemExtension>().ToList();

			_enhancementMatcherGroup = new ComponentMatcherGroup(new[] { typeof(ISubsystemEnhancement) });
		}

		public void Initialize()
		{
			foreach (var enhancedSystem in _enhancementMatcherGroup.MatchingEntities)
			{
				ExecuteExtensionAction(ese => ese.Initialize(enhancedSystem));
			}
		}

		private void ExecuteExtensionAction(Action<IEnhancementSystemExtension> action)
		{
			foreach (var extension in _enhancementSystemExtensions)
			{
				action(extension);
			}
		}
	}
}
