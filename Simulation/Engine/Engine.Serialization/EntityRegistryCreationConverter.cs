using System;
using Engine.Entities;
using Newtonsoft.Json.Converters;

namespace Engine.Serialization
{
	internal class EntityRegistryCreationConverter : CustomCreationConverter<IEntity>
	{
		private readonly IEntityRegistry _entityRegistry;

		public EntityRegistryCreationConverter(IEntityRegistry entityRegistry)
		{
			_entityRegistry = entityRegistry;
		}

		public override IEntity Create(Type objectType)
		{

			//if (_entityRegistry.TryGetEntityById())
			return null;
		}
	}
}
