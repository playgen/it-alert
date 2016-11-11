﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Behaviours
{
	public class NetworkRoutes : ReadOnlyProperty<Dictionary<int, Connection>>
	{
		protected NetworkRoutes(IEntity entity, Dictionary<int, Connection> value) 
			: base(entity, value)
		{
		}
	}
}
