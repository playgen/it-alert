using System;

namespace PlayGen.Engine.Serialization
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    public class SyncStateAttribute : Attribute
    {
        public StateLevel Levels { get; }

        public SyncStateAttribute(StateLevel levels)
        {
            Levels = levels;
        }
    }

	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Enum|AttributeTargets.Struct)]
	public class SyncStateType : Attribute
	{
		
	}
}
