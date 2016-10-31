using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace PlayGen.Engine.Serialization
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
