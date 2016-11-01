using System;
using Newtonsoft.Json.Serialization;

namespace Engine.Serialization
{
	internal class OrderedProperty : IComparable<OrderedProperty>
	{
		public int Order { get; set; }

		public JsonProperty Property { get; set; }
		public int CompareTo(OrderedProperty other)
		{
			return Order.CompareTo(other.Order);
		}
	}
}
