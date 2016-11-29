using System;

namespace Engine.Serialization
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class SyncStateAttribute : Attribute
	{
		public StateLevel Levels { get; private set; }

		public int Order { get; private set; }

		public SyncStateAttribute(StateLevel levels, int order = int.MaxValue)
		{
			Levels = levels;
			Order = order;
		}
	}
}
