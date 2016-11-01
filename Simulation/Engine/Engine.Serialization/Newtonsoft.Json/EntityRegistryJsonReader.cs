using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Engine.Serialization.Newtonsoft.Json
{
	internal class EntityRegistryJsonReader : JsonSerializerInternalReader
	{
		public EntityRegistryJsonReader(JsonSerializer serializer) 
			: base(serializer)
		{
		}

		#region base overrides

		#endregion
	}
}
