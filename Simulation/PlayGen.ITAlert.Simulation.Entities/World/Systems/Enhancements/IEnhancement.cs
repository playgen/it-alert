using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation.Entities.World.Systems.Enhancements
{
	public interface IEnhancement : IITAlertEntity<EnhancementState>
	{
		EnhancementType EnhancementType { get; }
	}
}
