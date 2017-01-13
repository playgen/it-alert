using System.Globalization;
using System.IO;
using System.Text;
using Engine.Entities;
using Engine.Serialization.Newtonsoft.Json;
using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;

namespace Engine.Serialization
{
	// ReSharper disable once InconsistentNaming
	public class ECSSerializer
	{
		private static JsonSerializerSettings GetDefaultSettings()
		{
			var settings = new JsonSerializerSettings();
			settings.Converters.Add(new StringEnumConverter());

			// ignore reference loops as these will be taken care of
			// settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; // there should be no more reference loops!
			
			// minimize the size of the output
			settings.DefaultValueHandling = DefaultValueHandling.Include;
			settings.NullValueHandling = NullValueHandling.Ignore;
			settings.Formatting = Formatting.None;
			
			// deal with references
			// settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor; // this should not be necessary if we resolve from DI
			// settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
			
			// polymorphism
			settings.TypeNameHandling = TypeNameHandling.Auto;
			
			// big graph
			settings.MaxDepth = int.MaxValue;


			return settings;
		}

		#region JsonConvert overrides

		private string SerializeObject(object value, JsonSerializerSettings settings)
		{
			var jsonSerializer = JsonSerializer.CreateDefault(settings);

			var sb = new StringBuilder(256);
			var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
			using (var jsonWriter = new JsonTextWriter(sw))
			{
				jsonWriter.Formatting = jsonSerializer.Formatting;

				jsonSerializer.Serialize(jsonWriter, value, null); // type);
			}

			return sw.ToString();
		}

		private T DeserializeObject<T>(string value, JsonSerializerSettings settings)
		{
			ValidationUtils.ArgumentNotNull(value, nameof(value));

			var jsonSerializer = new ECSJsonSerializer();

			JsonSerializer.ApplySerializerSettings(jsonSerializer, settings);

			// by default DeserializeObject should check for additional content
			if (!jsonSerializer.IsCheckAdditionalContentSet())
			{
				jsonSerializer.CheckAdditionalContent = true;
			}

			using (var reader = new JsonTextReader(new StringReader(value)))
			{
				return (T)jsonSerializer.Deserialize(reader, typeof(T));
			}
		}

		#endregion

		public static byte[] Serialize(ECS ecs)
		{
			var serializerSettings = GetDefaultSettings();

			var serializer = new ECSSerializer();
			var serializedString = serializer.SerializeObject(ecs.EntityRegistry.Entities.Values, serializerSettings);

			return Encoding.UTF8.GetBytes(serializedString);
		}

		public static Entity[] DeserializeEntities(ECS ecs, byte[] entityCollectionBytes)
		{
			var serializerSettings = GetDefaultSettings();
			serializerSettings.ContractResolver = new ECSContractResolver(ecs);
			
			var objString = Encoding.UTF8.GetString(entityCollectionBytes);
			var serializer = new ECSSerializer();
			var obj = serializer.DeserializeObject<Entity[]>(objString, serializerSettings);

			//ecs.OnDeserialized();
			return obj;
		}

		#region compression

		public static byte[] Compress(byte[] input)
		{
			using (var inputStream = new MemoryStream(input))
			using (var outputStream = new MemoryStream())
			{
				GZip.Compress(inputStream, outputStream, false, 5);
				return outputStream.ToArray();
			}
		}

		public static byte[] Decompress(byte[] input)
		{
			using (var inputStream = new MemoryStream(input))
			using (var outputStream = new MemoryStream())
			{
				GZip.Decompress(inputStream, outputStream, false);
				return outputStream.ToArray();
			}
		}

		#endregion
	}
}
