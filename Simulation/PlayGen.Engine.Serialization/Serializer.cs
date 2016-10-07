using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace PlayGen.Engine.Serialization
{
	public class Serializer
	{
		private static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings();

		static Serializer()
		{
			DefaultSettings.Converters.Add(new StringEnumConverter());
			// ignore reference loops as these will be taken care of
			DefaultSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			// minimize the size of the output
			DefaultSettings.DefaultValueHandling = DefaultValueHandling.Include;
			DefaultSettings.NullValueHandling = NullValueHandling.Ignore;
			DefaultSettings.Formatting = Formatting.None;
			// deal with references
			DefaultSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
			DefaultSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
			// polymorphism
			DefaultSettings.TypeNameHandling = TypeNameHandling.Auto;
			// big graph
			DefaultSettings.MaxDepth = int.MaxValue;
		}

		public static byte[] Serialize<T>(T obj)
			where T : ISerializable, IEntityRegistry
		{
			var serializerSettings = DefaultSettings;

			serializerSettings.ContractResolver = new SyncStateContractResolver(StateLevel.Full, obj);
			serializerSettings.ReferenceResolverProvider = () => new EntityRegistryReferenceResolver(obj);

			var serializedString = JsonConvert.SerializeObject(obj, serializerSettings);
			return Encoding.UTF8.GetBytes(serializedString);
		}

		public static T Deserialize<T>(byte[] simulationBytes)
			where T : ISerializable
		{
			var objString = Encoding.UTF8.GetString(simulationBytes);
			var obj = JsonConvert.DeserializeObject<T>(objString, DefaultSettings);
			obj.OnDeserialized();
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
