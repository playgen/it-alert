using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;

namespace PlayGen.ITAlert.Simulation.Systems
{
	public interface IEnhancement : IITAlertEntity<EnhancementState>
	{
		EnhancementType EnhancementType { get; }
	}
}
