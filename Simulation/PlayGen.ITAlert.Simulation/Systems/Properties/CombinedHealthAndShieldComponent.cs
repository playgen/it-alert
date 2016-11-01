using System;
using Engine.Components;
using Engine.Components.Property;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	[ComponentDependency(typeof(SystemHealth))]
	[ComponentDependency(typeof(SystemShield))]
	public class CombinedHealthAndShieldComponent : Property<int>
	{
		private SystemHealth _health;
		private SystemShield _shield;

		// lazy loaders
		private SystemHealth Health => _health ?? (_health = Container.GetComponent<SystemHealth>());
		private SystemShield Shield => _shield ?? (_shield = Container.GetComponent<SystemShield>());

		#region Constructors

		public CombinedHealthAndShieldComponent(IComponentContainer componentContainer, int initialValue = SimulationConstants.MaxShield)
			: base(componentContainer, "CombinedHealthAndShield", false, initialValue)
		{

		}

		#endregion

		public override void Set(int value)
		{
			var newValue = Math.Max(0, Math.Min(SimulationConstants.MaxHealth, value));
			base.Set(newValue);
		}

		protected override int GetValue()
		{
			return Health.Value + Shield.Value;
		}

		public override void ApplyDelta(int delta)
		{
			if (delta > 0)
			{
				if (Health.Value < Health.MaxValue)
				{
					var overflow = delta - (Health.MaxValue - Health.Value);
					Health.ApplyDelta(delta);
					if (overflow > 0)
					{
						Shield.ApplyDelta(overflow);
					}
				}
				else
				{
					Shield.ApplyDelta(delta);
				}
			}
			else
			{
				if (Shield.Value > 0)
				{
					var overflow = Shield.Value + delta;
					Shield.ApplyDelta(delta);
					if (overflow < 0)
					{
						Health.ApplyDelta(overflow);
					}
				}
				else
				{
					Health.ApplyDelta(delta);
				}
			}
		}
	}
}
