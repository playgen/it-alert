using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Components
{
	public interface IEmitState : IComponent
	{
		IComponentState GetState();
	}
}
