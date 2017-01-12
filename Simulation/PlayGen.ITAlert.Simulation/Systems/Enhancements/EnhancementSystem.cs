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

		#region constructor

		public EnhancementSystem(IComponentRegistry componentRegistry, 
			IEntityRegistry entityRegistry, 
			List<IEnhancementSystemExtension> enhancementSystemExtensions)
			: base(componentRegistry, entityRegistry)
		{
			_enhancementSystemExtensions = enhancementSystemExtensions;
			_enhancementMatcherGroup = componentRegistry.CreateMatcherGroup(new[] { typeof(ISubsystemEnhancement) });
		}

		#endregion

		#region IInitializingSystem

		public void Initialize()
		{
			foreach (var enhancedSystem in _enhancementMatcherGroup.MatchingEntities)
			{
				ExecuteExtensionAction(ese => ese.Initialize(enhancedSystem));
			}
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
