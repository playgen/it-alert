﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Engine.Core.Components;

namespace Engine.Components.Property
{
	public interface IEmitState : IComponent
	{
		object GetState();
	}

	public interface IEmitState<out TState> : IEmitState
	{

	}
}