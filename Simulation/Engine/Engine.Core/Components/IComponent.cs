using System;
using Engine.Entities;

namespace Engine.Components
{
	public interface IComponent : IDisposable
	{
		void Initialize(IEntity entity);
	}
}
