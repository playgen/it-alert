using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Common;
using Engine.Components.Property;
using Engine.Core.Components;

namespace Engine.Components.Common
{
	public class CartesianCoordinate2DProperty : Property<Vector>
	{
		public CartesianCoordinate2DProperty(IComponentContainer container, string propertyName, bool includeInState, Vector value) 
			: base(container, propertyName, includeInState, value)
		{
		}
	}
}
