using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation.World.Enhancements
{
	public interface IEnhancement : IITAlertEntity<EnhancementState>
	{
		EnhancementType EnhancementType { get; }
	}
}
