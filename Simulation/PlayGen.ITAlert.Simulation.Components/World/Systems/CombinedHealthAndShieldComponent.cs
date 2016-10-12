using System;
using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Property;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Entities.World.Systems.Components
{
	[ComponentDependency(typeof(SubsystemHealthComponent))]
	[ComponentDependency(typeof(SubsystemShieldComponent))]
	public class CombinedHealthAndShieldComponent : PropertyComponentBase<int>
	{
		private SubsystemHealthComponent _healthComponent;
		private SubsystemShieldComponent _shieldComponent;

		// lazy loaders
		private SubsystemHealthComponent HealthComponent => _healthComponent ?? (_healthComponent = EntityComponents.GetComponent<SubsystemHealthComponent>());
		private SubsystemShieldComponent ShieldComponent => _shieldComponent ?? (_shieldComponent = EntityComponents.GetComponent<SubsystemShieldComponent>());

		#region Constructors

		public CombinedHealthAndShieldComponent(IComponentContainer componentContainer, int initialValue = SimulationConstants.MaxShield)
			: base(componentContainer, "CombinedHealthAndShield", initialValue)
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
			return HealthComponent.Value + ShieldComponent.Value;
		}

		public override void ApplyDelta(int delta)
		{
			if (delta > 0)
			{
				if (HealthComponent.Value < HealthComponent.MaxValue)
				{
					var overflow = delta - (HealthComponent.MaxValue - HealthComponent.Value);
					HealthComponent.ApplyDelta(delta);
					if (overflow > 0)
					{
						ShieldComponent.ApplyDelta(overflow);
					}
				}
				else
				{
					ShieldComponent.ApplyDelta(delta);
				}
			}
			else
			{
				if (ShieldComponent.Value > 0)
				{
					var overflow = ShieldComponent.Value + delta;
					ShieldComponent.ApplyDelta(delta);
					if (overflow < 0)
					{
						HealthComponent.ApplyDelta(overflow);
					}
				}
				else
				{
					HealthComponent.ApplyDelta(delta);
				}
			}
		}
	}
}
