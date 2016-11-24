using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Common;
using Engine.Components.Property;
using Engine.Core.Components;
using Engine.Core.Entities;

namespace Engine.Components.Common
{
	public class Coordinate2DProperty : Property<Vector>
	{
		public Coordinate2DProperty(IEntity entity) 
			: base(entity)
		{
		}
	}
}
