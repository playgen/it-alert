using Engine.Components;
using Engine.Components.Property;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	[ComponentDependency(typeof(SystemHealth))]
	public class SystemIsDeadComponent : Property<bool>
	{
		private SystemHealth _health;

		// lazy loaders
		private SystemHealth Health => _health ?? (_health = Container.GetComponent<SystemHealth>());

		#region Constructors

		public SystemIsDeadComponent(IComponentContainer componentContainer)
			: base(componentContainer, "IsDead", true, false)
		{

		}

		#endregion

		protected override bool GetValue()
		{
			return Health.Value == Health.MinValue;
		}
	}
}
