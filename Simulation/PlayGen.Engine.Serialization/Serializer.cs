﻿using System;
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
			DefaultSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
			// polymorphism
			DefaultSettings.TypeNameHandling = TypeNameHandling.Auto;
			// big graph
			DefaultSettings.MaxDepth = int.MaxValue;

			// TODO: select level based resolver on usage
			// only serialize properties at this state transmission level
			DefaultSettings.ContractResolver = new SyncStateResolver(StateLevel.Full);
		}

		public static byte[] Serialize<T>(T obj)
			where T : ISerializable, IEntityRegistry
		{
			var serializerSettings = DefaultSettings;

			serializerSettings.ReferenceResolverProvider = () => new EntityRegistryReferenceResolver(obj);

			var simulationString = JsonConvert.SerializeObject(obj, serializerSettings);
			return Encoding.UTF8.GetBytes(simulationString);
		}

		public static T Deserialize<T>(byte[] simulationBytes)
			where T : ISerializable
		{
			var objString = Encoding.UTF8.GetString(simulationBytes);
			var obj = JsonConvert.DeserializeObject<T>(objString, DefaultSettings);
			obj.OnDeserialized();
			return obj;
		}

		#region contract resolution

		private class SyncStateResolver : DefaultContractResolver
		{
			private readonly Dictionary<Type, List<JsonProperty>> _propertyCache = new Dictionary<Type, List<JsonProperty>>();

			private StateLevel Level { get; }

			public SyncStateResolver(StateLevel level)
			{
				Level = level;
			}

			protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
			{
				List<JsonProperty> properties;
				if (_propertyCache.TryGetValue(type, out properties) == false)
				{
					var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

					var orderedProperties = new List<OrderedProperty>();

					foreach (var prop in props)
					{
						var syncStateAttribute =
							prop.GetCustomAttributes(true)
								.OfType<SyncStateAttribute>()
								.SingleOrDefault(ssa => Level.IncludesFlag(ssa.Levels));
						if (syncStateAttribute == null)
						{
							continue;
						}

						var jsonProperty = base.CreateProperty(prop, memberSerialization);
						jsonProperty.Writable = true;
						jsonProperty.Readable = true;
						orderedProperties.Add(new OrderedProperty() {
							Property = jsonProperty,
							Order = syncStateAttribute.Order
						});
					}

					foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
					{
						var syncStateAttribute =
							field.GetCustomAttributes(true)
								.OfType<SyncStateAttribute>()
								.SingleOrDefault(ssa => Level.IncludesFlag(ssa.Levels));
						if (syncStateAttribute == null)
						{
							continue;
						}
						var jsonProperty = base.CreateProperty(field, memberSerialization);
						jsonProperty.Writable = true;
						jsonProperty.Readable = true;
						orderedProperties.Add(new OrderedProperty()
						{
							Property = jsonProperty,
							Order = syncStateAttribute.Order
						});
					}

					orderedProperties.Sort();
					properties = orderedProperties.Select(op => op.Property).ToList();
					_propertyCache.Add(type, properties);
				}
				return properties;
			}
		}

		#endregion

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
