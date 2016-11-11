using Engine.Components;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	[ComponentDependency(typeof(SystemHealth))]
	public class SystemIsDeadComponent : Property<bool>
	{
		private SystemHealth _health;

		// lazy loaders
		private SystemHealth Health => _health ?? (_health = Entity.GetComponent<SystemHealth>());

		#region Constructors

		public SystemIsDeadComponent(IEntity entity)
			: base(entity, "IsDead", true, false)
		{

		}

		#endregion

		protected override bool GetValue()
		{
			return Health.Value == Health.MinValue;
		}
	}
}
