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
	public class CartesianCoordinate2DProperty : Property<Vector>
	{
		public CartesianCoordinate2DProperty(IEntity entity, Vector value) 
			: base(entity, value)
		{
		}
	}
}
