using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Components
{
	public interface IEmitState : IComponent
	{
		object GetState();
	}

	public interface IEmitState<out TState> : IEmitState
	{
		TState GetState();
	}
}
