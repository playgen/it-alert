namespace PlayGen.ITAlert.Simulation.Modules.Resources.Systems
{
	//public class VisitorsConsumeMemoryEffect : ISubsystemResourceEffect
	//{
	//	private readonly IEntityRegistry _entityRegistry;

	//	public VisitorsConsumeMemoryEffect(IEntityRegistry entityRegistry)
	//	{
	//		_entityRegistry = entityRegistry;
	//	}

	//	public void Tick()
	//	{
	//		MemoryResource memoryResource;
	//		Visitors visitors;
	//		if (subsystem.TryGetComponent(out memoryResource)
	//			&& subsystem.TryGetComponent(out visitors))
	//		{
	//			var sum = 0;
	//			foreach (var visitorId in visitors.Values)
	//			{
	//				Entity visitor;
	//				if (_entityRegistry.TryGetEntityById(visitorId, out visitor))
	//				{
	//					ConsumeMemory consumeMemory;
	//					if (visitor.TryGetComponent(out consumeMemory))
	//					{
	//						sum += consumeMemory.Value;
	//					}
	//				}
	//			}
	//			memoryResource.Value = sum;
	//		}
	//	}

	//}
}
