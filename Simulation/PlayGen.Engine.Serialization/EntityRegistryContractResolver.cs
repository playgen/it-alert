﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace PlayGen.Engine.Serialization
{
	internal class EntityRegistryContractResolver : DefaultContractResolver
	{

		private readonly Dictionary<Type, List<JsonProperty>> _propertyCache = new Dictionary<Type, List<JsonProperty>>();

		private readonly IEntityRegistry _entityRegistry;

		private StateLevel Level { get; }


		private readonly List<Action> _entityReferenceResolverQueue = new List<Action>();

		public EntityRegistryContractResolver(StateLevel level, IEntityRegistry registry)
		{
			_entityRegistry = registry;
			Level = level;
		}

		public override JsonContract ResolveContract(Type type)
		{
			var contract = base.ResolveContract(type);

			if (type.IsEntity())
			{
				//contract.IsReference = type.IsEntity();

			}

			return contract;
		}

		protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
		{
			return base.CreateDictionaryContract(objectType);
		}

		protected override JsonArrayContract CreateArrayContract(Type objectType)
		{
			var contract =  base.CreateArrayContract(objectType);

			//if (objectType.IsEntityCollection())
			//{
			//	contract.It
			//}

			return contract;
		}

		private void AttachEntityValueProviderDeserializationCallback(EntityRegistryValueProvider valueProvider)
		{
			valueProvider.DeserializingEntityReference += ValueProviderOnDeserializingEntityReference;
		}

		private void ValueProviderOnDeserializingEntityReference(object sender, DeserializingEntityReferenceEventArgs deserializingEntityReferenceEventArgs)
		{
			_entityReferenceResolverQueue.Add(deserializingEntityReferenceEventArgs.Setter);
		}

		public void ResolveEntityReferences()
		{
			foreach (var entityReferenceResolveAction in _entityReferenceResolverQueue)
			{
				entityReferenceResolveAction();
			}
			_entityReferenceResolverQueue.Clear();
		}


		#region property resolver

		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			List<JsonProperty> properties;
			if (_propertyCache.TryGetValue(type, out properties) == false)
			{
				var typeIsEntity = type.IsEntity();
				var orderedProperties = new List<OrderedProperty>();
				
				var currentType = type;
				while (currentType != null && currentType != typeof(object))
				{
					var props = currentType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
					
					foreach (var propertyInfo in props)
					{
						var syncStateAttribute =
							propertyInfo.GetCustomAttributes(true)
								.OfType<SyncStateAttribute>()
								.SingleOrDefault(ssa => Level.IncludesFlag(ssa.Levels));
						if (syncStateAttribute == null)
						{
							continue;
						}

						var jsonProperty = base.CreateProperty(propertyInfo, memberSerialization);
						jsonProperty.Writable = true;
						jsonProperty.Readable = true;

						if ((propertyInfo.PropertyType.IsEntityCollection() || propertyInfo.PropertyType.IsEntity()) 
							&& type.IsEntityRegistry() == false)
						{
							jsonProperty.ValueProvider = new EntityRegistryValueProvider(_entityRegistry, propertyInfo);
						}

						orderedProperties.Add(new OrderedProperty()
						{
							Property = jsonProperty,
							Order = syncStateAttribute.Order
						});
					}

					foreach (var fieldInfo in currentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
					{
						var syncStateAttribute = fieldInfo.GetCustomAttributes(true)
								.OfType<SyncStateAttribute>()
								.SingleOrDefault(ssa => Level.IncludesFlag(ssa.Levels));
						if (syncStateAttribute == null)
						{
							continue;
						}
						var jsonProperty = base.CreateProperty(fieldInfo, memberSerialization);
						jsonProperty.Writable = true;
						jsonProperty.Readable = true;

						if (fieldInfo.FieldType.IsEntity() && type.IsEntityRegistry() == false)
						{
							jsonProperty.ValueProvider = new EntityRegistryValueProvider(_entityRegistry, fieldInfo);
						}

						orderedProperties.Add(new OrderedProperty()
						{
							Property = jsonProperty,
							Order = syncStateAttribute.Order
						});
					}
					currentType = currentType.BaseType;
				}
				orderedProperties.Sort();
				properties = orderedProperties.Select(op => op.Property).ToList();
				_propertyCache.Add(type, properties);
			}
			return properties;
		}

		#endregion
	}
}
