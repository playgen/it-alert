namespace PlayGen.ITAlert.Simulation.Common
{
	//TODO: This probably wont scale very well once we get more extensible, but it's only temporary
	public enum EntityType
	{
		Undefined = 0,

		Subsystem,
		Connection,

		Enhancement,

		Player,
		Npc,

		Item,

	}
}
