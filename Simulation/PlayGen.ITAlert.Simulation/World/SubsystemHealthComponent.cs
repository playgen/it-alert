using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Components;
using PlayGen.ITAlert.Common;

namespace PlayGen.ITAlert.Simulation.World
{
	internal class SubsystemHealthComponent : ComponentBase, IPropertyComponent<int>
	{
		public string PropertyName { get; } = "Health";

		public int Value { get; private set; }

		private readonly GetComponentContainerDelegate _componentResolver;

		#region Constructors

		public SubsystemHealthComponent(GetComponentContainerDelegate componentResolver, int initialValue)
			: base(new Type[] { typeof(SubsystemShieldComponent) })
		{
			_componentResolver = componentResolver;
			Value = initialValue;
		}

		#endregion

		public void Set(int value)
		{
			Value = Math.Max(0, Math.Min(SimulationConstants.MaxHealth, value));
		}

		public void ApplyDelta(int delta)
		{
			Set(Value + delta);
		}
	}
}
