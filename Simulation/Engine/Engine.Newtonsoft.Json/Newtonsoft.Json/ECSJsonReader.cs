using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Engine.Serialization.Newtonsoft.Json
{
	internal class ECSJsonReader : JsonSerializerInternalReader
	{
		public ECSJsonReader(JsonSerializer serializer) 
			: base(serializer)
		{
		}

		#region base overrides

		#endregion
	}
}
