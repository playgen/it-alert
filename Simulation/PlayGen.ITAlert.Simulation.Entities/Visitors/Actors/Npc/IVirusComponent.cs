using PlayGen.Engine.Components;
using PlayGen.ITAlert.Simulation.Entities.Visitors.Actors.Npc;
using PlayGen.ITAlert.Simulation.Entities.World.Systems;

namespace PlayGen.ITAlert.Simulation.Components.Visitors.Actors.Npc
{
	interface IVirusComponent : IComponent
	{
		void OnTickOnSubsystem(Virus virus, Subsystem subsystem);
	}
}
