using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Serialization
{
	public interface ISerializable
	{
		void OnDeserialized();
	}
}
