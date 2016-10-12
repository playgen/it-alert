using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Entities.World.Systems.Components
{
	[ComponentDependency(typeof(SubsystemHealthComponent))]
	public class SubsystemIsDeadComponent : PropertyComponentBase<bool>
	{
		private SubsystemHealthComponent _healthComponent;

		// lazy loaders
		private SubsystemHealthComponent HealthComponent => _healthComponent ?? (_healthComponent = EntityComponents.GetComponent<SubsystemHealthComponent>());

		#region Constructors

		public SubsystemIsDeadComponent(IComponentContainer componentContainer)
			: base(componentContainer, "CombinedHealthAndShield", false)
		{

		}

		#endregion

		protected override bool GetValue()
		{
			return HealthComponent.Value == HealthComponent.MinValue;
		}
	}
}
