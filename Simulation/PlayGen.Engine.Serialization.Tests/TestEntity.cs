using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Serialization.Tests
{
	public class TestEntity : ITestEntity
	{
		[SyncState(StateLevel.Full)]
		public int Id { get; }

		public event EventHandler EntityDestroyed;

		[SyncState(StateLevel.Full)]
		public ITestEntity OtherEntity { get; set; }

		public TestEntity(IEntityRegistry registry)
		{
			Id = registry.EntitySeed;
		}

		public void OnDeserialized()
		{
			
		}

		public void Dispose()
		{
			
		}

	}
}
