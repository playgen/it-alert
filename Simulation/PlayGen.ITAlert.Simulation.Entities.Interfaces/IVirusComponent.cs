namespace PlayGen.ITAlert.Simulation.Entities.Interfaces
{
	interface IVirusComponent : IComponent
	{
		void OnTickOnSubsystem(Virus virus, Subsystem subsystem);
	}
}
