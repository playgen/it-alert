using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using PlayGen.Engine.Entities;

namespace PlayGen.Engine.Serialization
{
	public class EntityRegistryReferenceResolver : IReferenceResolver
	{
		private IEntityRegistry _registry;

		private BidirectionalDictionary<string, object> _nonEntityReferences = new BidirectionalDictionary<string, object>();

		private int _referenceCount;

		public EntityRegistryReferenceResolver(IEntityRegistry registry)
		{
			_registry = registry;
		}

		public object ResolveReference(object context, string reference)
		{
			throw new System.NotImplementedException();
		}

		public string GetReference(object context, object value)
		{
			var entity = value as IEntity;
			string reference;
			if (entity == null)
			{
				if (!_nonEntityReferences.TryGetBySecond(value, out reference))
				{
					_referenceCount--;
					reference = _referenceCount.ToString(CultureInfo.InvariantCulture);
					_nonEntityReferences.Set(reference, value);
				}
			}
			else
			{
				reference = entity.Id.ToString(CultureInfo.InvariantCulture);
			}
			return reference;
		}

		public bool IsReferenced(object context, object value)
		{
			var entity = value as IEntity;
			if (entity == null)
			{
				string reference;
				return _nonEntityReferences.TryGetBySecond(value, out reference);
			}
			if (_registry.TryGetEntityById(entity.Id, out entity))
			{

			}
			return false;
		}

		public void AddReference(object context, string reference, object value)
		{
			throw new System.NotImplementedException();
		}
	}
}