using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	public class MatchAll : ComponentMatcher
	{
		public override bool IsMatch(Entity entity)
		{
			return true;
		}
	}
}
