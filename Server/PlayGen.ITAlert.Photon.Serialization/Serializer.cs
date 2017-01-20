using System.Text;
using Engine.Serialization;
using Newtonsoft.Json;

namespace PlayGen.ITAlert.Photon.Serialization
{
	public static class Serializer
	{
		#region extract 
		// TODO: candidate for move to playgen.photon
		// extend messsage deserialization to support configurable serializeration handlers per message type?

		public static byte[] Serialize(object content)
		{
			var serialziedString = JsonConvert.SerializeObject(content,
				Formatting.None,
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All
				});
			 
			var bytes = Encoding.UTF8.GetBytes(serialziedString);
			var compressedBytes = CompressionUtil.Compress(bytes);
			return compressedBytes;
		}

		public static T Deserialize<T>(byte[] compressedBytes)
		{
			// TODO: should not use ecs specific compressor when extracted
			var decompressedBytes = CompressionUtil.Decompress(compressedBytes); 
			var serializedString = Encoding.UTF8.GetString(decompressedBytes);
			var deserializedObject = JsonConvert.DeserializeObject<T>(serializedString,
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				});

			return deserializedObject;
		}

		#endregion


	}
}
