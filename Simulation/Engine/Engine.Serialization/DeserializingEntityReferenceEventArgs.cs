using System;

namespace Engine.Serialization
{
	internal class DeserializingEntityReferenceEventArgs : EventArgs
	{
		public Action Setter { get; }

		public DeserializingEntityReferenceEventArgs(Action setter)
		{
			Setter = setter;
		}
	}
}
