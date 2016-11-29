using System;
using Engine.Components;
using Engine.Messaging;
using Engine.Serialization;

namespace Engine.Entities
{
	// TODO: the dependency on Rx-Main can be broken in Engine.Entites and Simulation.Entities when we upgrade to .NET 4 and IObservable is included in System

	public interface IEntity : IMessageHub, IComponentContainer, ISerializable, IDisposable
	{
		int Id { get; }

		event EventHandler EntityDestroyed;
	}
}
