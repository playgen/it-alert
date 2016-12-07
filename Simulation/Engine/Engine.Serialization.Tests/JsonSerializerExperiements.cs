//using Engine.Entities;

//namespace Engine.Serialization.Tests
//{
//	public class JsonSerializerExperiements
//	{
//		public static void TestReferenceResolution()
//		{
//			var registry = new EntityRegistry();

//			var entityA = new Entity(registry);
//			registry.AddEntity(entityA);

//			var entityB = new Entity(registry);
//			registry.AddEntity(entityB);

//			entityA.OtherEntity = entityB;
//			entityB.OtherEntity = entityA;

//			var result = ECSSerializer.Serialize(registry);
//		}
//	}
//}
