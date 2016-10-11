using System;

namespace PlayGen.Engine.Components
{
    public interface IComponent
    {
		Type[] ComponentDependencies { get; }
	}
}
