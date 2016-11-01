using System.Globalization;
using Engine.Serialization.Serialization;

namespace Engine.Serialization
{
	internal class DefaultReferenceResolver : IReferenceResolver
	{
		private int _referenceCount;

		private BidirectionalDictionary<string, object> _nonEntityReferences = new BidirectionalDictionary<string, object>();


		private BidirectionalDictionary<string, object> GetMappings(object context)
		{
			return _nonEntityReferences;
		}

		public object ResolveReference(object context, string reference)
		{
			object value;
			GetMappings(context).TryGetByFirst(reference, out value);
			return value;
		}

		public string GetReference(object context, object value)
		{
			var mappings = GetMappings(context);

			string reference;
			if (!mappings.TryGetBySecond(value, out reference))
			{
				_referenceCount++;
				reference = _referenceCount.ToString(CultureInfo.InvariantCulture);
				mappings.Set(reference, value);
			}

			return reference;
		}

		public void AddReference(object context, string reference, object value)
		{
			GetMappings(context).Set(reference, value);
		}

		public bool IsReferenced(object context, object value)
		{
			string reference;
			return GetMappings(context).TryGetBySecond(value, out reference);
		}
	}
}
