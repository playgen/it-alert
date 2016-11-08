﻿using System;
using Engine.Core.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Engine.Serialization
{
	internal class EntityCreationConverter : CustomCreationConverter<IEntity>
	{
		private readonly IEntityRegistry _entityRegistry;

		public EntityCreationConverter(IEntityRegistry entityRegistry)
		{
			_entityRegistry = entityRegistry;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}

			IEntity value = Create(objectType);
			if (value == null)
			{
				throw new JsonSerializationException("No object created.");
			}

			serializer.Populate(reader, value);
			return value;
		}

		public override IEntity Create(Type objectType)
		{
			//IEntity entity;
			//_entityRegistry.TryGetEntityById()
			return null;
		}
	}
}