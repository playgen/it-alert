using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	public class FlagComponent : Component
	{
		public FlagComponent(IEntity entity) 
			: base(entity)
		{
		}
	}
}
