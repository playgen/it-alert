using System;
using Engine.Components;
using Engine.Components.Property;
using Engine.Core.Entities;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	[ComponentDependency(typeof(SystemHealth))]
	[ComponentDependency(typeof(SystemShield))]
	public class CombinedHealthAndShield : Property<int>
	{
		private SystemHealth _health;
		private SystemShield _shield;

		// lazy loaders
		private SystemHealth Health => _health ?? (_health = Entity.GetComponent<SystemHealth>());
		private SystemShield Shield => _shield ?? (_shield = Entity.GetComponent<SystemShield>());

		#region Constructors

		public CombinedHealthAndShield(IEntity entity, int initialValue = SimulationConstants.MaxShield)
			: base(entity, initialValue)
		{

		}

		#endregion

		public override void SetValue(int value)
		{
			var newValue = Math.Max(0, Math.Min(SimulationConstants.MaxHealth, value));
			base.SetValue(newValue);
		}

		protected override int GetValue()
		{
			return Health.Value + Shield.Value;
		}

		public void ApplyDelta(int delta)
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
