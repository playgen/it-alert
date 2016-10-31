using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Serialization
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
