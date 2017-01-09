using System;
using Engine.Components;
using Engine.Entities;
//using Subsystem.Reactive.Disposables;

namespace PlayGen.ITAlert.Simulation.Components.Movement
{
	public class VisitorPosition : IComponent
	{
		public int CurrentTick { get; private set; }

		public int Position { get; private set; }

		// TODO: dereference and store just id
		public Entity Host { get; private set; }

		//private IDisposable _visitorSubscription;

		public void SetHost(Entity host)
		{
			Host = host;
			//_visitorSubscription?.Dispose();
			//_visitorSubscription = new CompositeDisposable
			//{
			//	Host.Subscribe(Entity),
			//	Entity.Subscribe(Host),
			//};
		}

		/// <summary>
		/// Provide a tick safe method to update the current position
		/// </summary>
		/// <param name="position"></param>
		/// <param name="currentTick"></param>
		/// <returns>True if the position was updated. False if this position has been updated already this tick.</returns>
		public bool SetPosition(int position, int currentTick)
		{
			if (currentTick > CurrentTick)
			{
				Position = position;
				CurrentTick = currentTick;
				return true;
			}
			return false;
		}
	}
}
