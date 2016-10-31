using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using PlayGen.Engine.Entities;
using PlayGen.Engine.Serialization.Newtonsoft.Json;

namespace PlayGen.Engine.Serialization
{
	public class EntityRegistrySerializer
	{
		private static JsonSerializerSettings GetDefaultSettings()
		{
			var settings = new JsonSerializerSettings();
			settings.Converters.Add(new StringEnumConverter());
			// ignore reference loops as these will be taken care of
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			// minimize the size of the output
			settings.DefaultValueHandling = DefaultValueHandling.Include;
			settings.NullValueHandling = NullValueHandling.Ignore;
			settings.Formatting = Formatting.None;
			// deal with references
			settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
			settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
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

			var jsonSerializer = new EntityRegistryJsonSerializer();

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

		public static byte[] Serialize<T>(T obj)
			where T : ISerializable, IEntityRegistry
		{
			var serializerSettings = GetDefaultSettings();
			serializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

			var serializedString = JsonConvert.SerializeObject(obj, serializerSettings);

			return Encoding.UTF8.GetBytes(serializedString);
		}

		public byte[] SerializeDifferential<T>(T obj)
			where T : ISerializable, IEntityRegistry
		{
			var serializerSettings = GetDefaultSettings();

			var contractResolver = new EntityRegistryContractResolver(StateLevel.Differential, obj);
			serializerSettings.ContractResolver = contractResolver;

			//serializerSettings.ReferenceResolverProvider = () => new EntityRegistryReferenceResolver(obj);

			var serializedString = SerializeObject(obj, serializerSettings);

			return Encoding.UTF8.GetBytes(serializedString);
		}

		public static T Deserialize<T>(byte[] simulationBytes)
			where T : ISerializable
		{
			var serializerSettings = GetDefaultSettings();

			var objString = Encoding.UTF8.GetString(simulationBytes);
			var obj = JsonConvert.DeserializeObject<T>(objString, serializerSettings);

			obj.OnDeserialized();
			return obj;
		}

		public void DeserializeDifferential<T>(byte[] simulationBytes, T entityRegistry)
			where T : ISerializable, IEntityRegistry
		{
			var serializerSettings = GetDefaultSettings();

			var contractResolver = new EntityRegistryContractResolver(StateLevel.Full, entityRegistry);
			serializerSettings.ContractResolver = contractResolver;

			var objString = Encoding.UTF8.GetString(simulationBytes);
			var obj = DeserializeObject<T>(objString, serializerSettings);

			contractResolver.ResolveEntityReferences();
			// onDeserialized call is not necessary when process has been done differntially
			//obj.OnDeserialized();
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
