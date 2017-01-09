using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Components
{
	public class MatchAll : ComponentMatcher
	{
		public MatchAll(IEnumerable<Type> requiredTypes)
			: base(new Type[0])
		{
		}
	}
}
