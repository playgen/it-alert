using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Systems
{
	/// <summary>
	/// This is the base interface for all system extensions
	/// System extensions provide a mechanism for extending the behaviour of specific systems with new logic.
	/// System extensions are either called once for every entity priocessed by the parent system, or located by evaluating an expression again the specific entity
	/// TODO: provide base class method for implementing every, or key based extension location
	/// </summary>
	public interface ISystemExtension
	{
	}
}
