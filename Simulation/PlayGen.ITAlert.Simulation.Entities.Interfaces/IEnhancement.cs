namespace PlayGen.ITAlert.Simulation.Entities.Interfaces
{
	public interface IEnhancement : IITAlertEntity<EnhancementState>
	{
		EnhancementType EnhancementType { get; }
	}
}
