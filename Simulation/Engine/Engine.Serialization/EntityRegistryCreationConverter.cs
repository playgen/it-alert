using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using PlayGen.Engine.Entities;

namespace PlayGen.Engine.Serialization
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
