using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Serialization.Tests
{
	public class JsonSerializerExperiements
	{
		public static void TestReferenceResolution()
		{
			var registry = new TestEntityRegistry();

			var entityA = new TestEntity(registry);
			registry.AddEntity(entityA);

			var entityB = new TestEntity(registry);
			registry.AddEntity(entityB);

			entityA.OtherEntity = entityB;
			entityB.OtherEntity = entityA;

			var result = EntityRegistrySerializer.Serialize(registry);
		}
	}
}
