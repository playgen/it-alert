using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Components;
using PlayGen.ITAlert.Common;

namespace PlayGen.ITAlert.Simulation.World
{
	internal class SubsystemShieldComponent : ComponentBase, IPropertyComponent<int>
	{
		public string PropertyName { get; } = "Shield";

		public int Value { get; private set; }

		private readonly Func<IComponent> _componentResolver;

		#region Constructors

		public SubsystemShieldComponent(Func<IComponent> componentResolver, int initialValue)
			: base(new Type[] {typeof(SubsystemHealthComponent)})
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
