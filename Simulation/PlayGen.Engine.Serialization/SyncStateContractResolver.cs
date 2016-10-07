using System;
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

	internal class SyncStateContractResolver : DefaultContractResolver
	{
		private readonly Dictionary<Type, List<JsonProperty>> _propertyCache = new Dictionary<Type, List<JsonProperty>>();

		private readonly IEntityRegistry _entityRegistry;

		private StateLevel Level { get; }

		public SyncStateContractResolver(StateLevel level, IEntityRegistry registry)
		{
			_entityRegistry = registry;
			Level = level;
		}

		public override JsonContract ResolveContract(Type type)
		{
			var contract = base.ResolveContract(type);

			//contract.IsReference = type.IsEntity();

			return contract;

		}

		private class EntityRegistryValueProvider : IValueProvider
		{
			private readonly IEntityRegistry _entityRegistry;
			private readonly MemberInfo _memberInfo;

			private Func<object, object> _getter;
			private Action<object, object> _setter;

			public EntityRegistryValueProvider(IEntityRegistry entityRegistry, MemberInfo memberInfo)
			{
				_entityRegistry = entityRegistry;
				_memberInfo = memberInfo;
			}
			
			public void SetValue(object target, object value)
			{
				try
				{
					if (_setter == null)
					{
						_setter = CreateSet<object>(_memberInfo);
					}

					var entityId = (int) value;

					//TODO: investigate if we need to defer these until the entity registry has been deserialized
					IEntity entity;
					if (_entityRegistry.TryGetEntityById(entityId, out entity))
					{
						_setter(target, entity);
					}
					else
					{
						
					}
				}
				catch (Exception ex)
				{
					throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Error setting value to '{0}' on '{1}'.", _memberInfo.Name, target.GetType()), ex);
				}
			}

			public object GetValue(object target)
			{
				try
				{
					if (_getter == null)
					{
						_getter = CreateGet<object>(_memberInfo);
					}

					var value = _getter(target);

					if (value != null)
					{
						var entity = value as IEntity;
						if (entity == null)
						{
							throw new InvalidOperationException("value object is not an entity");
						}
						return entity.Id;
					}
					return null;
				}
				catch (Exception ex)
				{
					throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Error getting value from '{0}' on '{1}'.", _memberInfo.Name, target.GetType()), ex);
				}
			}

			#region Delegate Factory

			// extracted from Newtonsoft DynamicValueProvider, DynamicReflectionDelegateFactory, ReflectionDelegateFactory

			#region Get

			private static Func<T, object> CreateGet<T>(MemberInfo memberInfo)
			{
				PropertyInfo propertyInfo = memberInfo as PropertyInfo;
				if (propertyInfo != null)
				{
					return CreateGet<T>(propertyInfo);
				}

				FieldInfo fieldInfo = memberInfo as FieldInfo;
				if (fieldInfo != null)
				{
					return CreateGet<T>(fieldInfo);
				}

				throw new Exception(string.Format(CultureInfo.InvariantCulture, "Could not create getter for {0}.", memberInfo));
			}

			private static Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
			{
				DynamicMethod dynamicMethod = CreateDynamicMethod("Get" + propertyInfo.Name, typeof(object), new[] { typeof(T) }, propertyInfo.DeclaringType);
				ILGenerator generator = dynamicMethod.GetILGenerator();

				GenerateCreateGetPropertyIL(propertyInfo, generator);

				return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
			}

			private static void GenerateCreateGetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
			{
				MethodInfo getMethod = propertyInfo.GetGetMethod(true);
				if (getMethod == null)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,"Property '{0}' does not have a getter.", propertyInfo.Name));
				}

				if (!getMethod.IsStatic)
				{
					generator.PushInstance(propertyInfo.DeclaringType);
				}

				generator.CallMethod(getMethod);
				generator.BoxIfNeeded(propertyInfo.PropertyType);
				generator.Return();
			}

			private static Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
			{
				if (fieldInfo.IsLiteral)
				{
					object constantValue = fieldInfo.GetValue(null);
					Func<T, object> getter = o => constantValue;
					return getter;
				}

				DynamicMethod dynamicMethod = CreateDynamicMethod("Get" + fieldInfo.Name, typeof(T), new[] { typeof(object) }, fieldInfo.DeclaringType);
				ILGenerator generator = dynamicMethod.GetILGenerator();

				GenerateCreateGetFieldIL(fieldInfo, generator);

				return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
			}

			private static void GenerateCreateGetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
			{
				if (!fieldInfo.IsStatic)
				{
					generator.PushInstance(fieldInfo.DeclaringType);
					generator.Emit(OpCodes.Ldfld, fieldInfo);
				}
				else
				{
					generator.Emit(OpCodes.Ldsfld, fieldInfo);
				}

				generator.BoxIfNeeded(fieldInfo.FieldType);
				generator.Return();
			}

			#endregion

			#region set

			private static Action<T, object> CreateSet<T>(MemberInfo memberInfo)
			{
				PropertyInfo propertyInfo = memberInfo as PropertyInfo;
				if (propertyInfo != null)
				{
					return CreateSet<T>(propertyInfo);
				}

				FieldInfo fieldInfo = memberInfo as FieldInfo;
				if (fieldInfo != null)
				{
					return CreateSet<T>(fieldInfo);
				}

				throw new Exception(string.Format(CultureInfo.InvariantCulture, "Could not create setter for {0}.", memberInfo));
			}

			private static Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
			{
				DynamicMethod dynamicMethod = CreateDynamicMethod("Set" + fieldInfo.Name, null, new[] {typeof(T), typeof(object)}, fieldInfo.DeclaringType);
				ILGenerator generator = dynamicMethod.GetILGenerator();

				GenerateCreateSetFieldIL(fieldInfo, generator);

				return (Action<T, object>) dynamicMethod.CreateDelegate(typeof(Action<T, object>));
			}

			private static void GenerateCreateSetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
			{
				if (!fieldInfo.IsStatic)
				{
					generator.PushInstance(fieldInfo.DeclaringType);
				}

				generator.Emit(OpCodes.Ldarg_1);
				generator.UnboxIfNeeded(fieldInfo.FieldType);

				if (!fieldInfo.IsStatic)
				{
					generator.Emit(OpCodes.Stfld, fieldInfo);
				}
				else
				{
					generator.Emit(OpCodes.Stsfld, fieldInfo);
				}

				generator.Return();
			}

			private static Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
			{
				DynamicMethod dynamicMethod = CreateDynamicMethod("Set" + propertyInfo.Name, null, new[] {typeof(T), typeof(object)}, propertyInfo.DeclaringType);
				ILGenerator generator = dynamicMethod.GetILGenerator();

				GenerateCreateSetPropertyIL(propertyInfo, generator);

				return (Action<T, object>) dynamicMethod.CreateDelegate(typeof(Action<T, object>));
			}

			private static void GenerateCreateSetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
			{
				MethodInfo setMethod = propertyInfo.GetSetMethod(true);
				if (!setMethod.IsStatic)
				{
					generator.PushInstance(propertyInfo.DeclaringType);
				}

				generator.Emit(OpCodes.Ldarg_1);
				generator.UnboxIfNeeded(propertyInfo.PropertyType);
				generator.CallMethod(setMethod);


			}

			#endregion

			private static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
			{
				DynamicMethod dynamicMethod = !owner.IsInterface()
					? new DynamicMethod(name, returnType, parameterTypes, owner, true)
					: new DynamicMethod(name, returnType, parameterTypes, owner.Module, true);

				return dynamicMethod;
			}

			#endregion
		}

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

						if (typeIsEntity && propertyInfo.PropertyType.IsEntity())
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

						if (typeIsEntity && fieldInfo.FieldType.IsEntity())
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

	}
}
