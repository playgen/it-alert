using System;
using Engine.Entities;
using Newtonsoft.Json.Converters;

namespace Engine.Serialization
{
	internal class EntityRegistryCreationConverter : CustomCreationConverter<Entity>
	{
		private readonly EntityRegistry _entityRegistry;

		public EntityRegistryCreationConverter(EntityRegistry entityRegistry)
		{
			_entityRegistry = entityRegistry;
		}

		public override Entity Create(Type objectType)
		{

			//if (_entityRegistry.TryGetEntityById())
			return null;
		}
	}
}
