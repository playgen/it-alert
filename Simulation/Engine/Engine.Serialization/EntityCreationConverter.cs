using System;
using Engine.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Engine.Serialization
{
	internal class EntityCreationConverter : CustomCreationConverter<Entity>
	{
		private readonly EntityRegistry _entityRegistry;

		public EntityCreationConverter(EntityRegistry entityRegistry)
		{
			_entityRegistry = entityRegistry;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}

			Entity value = Create(objectType);
			if (value == null)
			{
				throw new JsonSerializationException("No object created.");
			}

			serializer.Populate(reader, value);
			return value;
		}

		public override Entity Create(Type objectType)
		{
			//Entity entity;
			//_entityRegistry.TryGetEntityById()
			return null;
		}
	}
}
