using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PlayGen.ITAlert.Common.Serialization;

namespace PlayGen.ITAlert.Simulation.Serialization
{
	public class SimulationSerializer
	{
		private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();

		static  SimulationSerializer()
		{
			SerializerSettings.Converters.Add(new StringEnumConverter());
			// ignore reference loops as these will be taken care of
			SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			// minimize the size of the output
			SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
			SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
			SerializerSettings.Formatting = Formatting.None;
			// deal with references
			SerializerSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
			SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
			// polymorphism
			SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
			// big graph
			SerializerSettings.MaxDepth = int.MaxValue;
			
			// TODO: select level based resolver on usage
			// only serialize properties at this state transmission level
			SerializerSettings.ContractResolver = new SyncStateResolver(StateLevel.Full);
		}

		public static byte[] SerializeSimulation(Simulation simulation)
		{
			var simulationString = JsonConvert.SerializeObject(simulation, SerializerSettings);
			return Encoding.UTF8.GetBytes(simulationString);
		}

		public static Simulation DeserializeSimulation(byte[] simulationBytes)
		{
			var simulationString = Encoding.UTF8.GetString(simulationBytes);
			var simulation = JsonConvert.DeserializeObject<Simulation>(simulationString, SerializerSettings);
			simulation.OnDeserialized();
			return simulation;
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

					properties = new List<JsonProperty>();

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
						properties.Add(jsonProperty);
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
						properties.Add(jsonProperty);
					}

					_propertyCache.Add(type, properties);
				}
				return properties;
			}
		}

		#endregion

		#region compression

		public static byte[] Compress(byte[] input)
		{
			// todo implement
			return input;
		}

		public static byte[] Decompress(byte[] input)
		{
			// todo implement
			return input;
		}

		#endregion
	}
}
