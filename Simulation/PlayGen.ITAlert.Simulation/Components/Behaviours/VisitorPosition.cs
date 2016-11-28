using System;
using System.Reactive.Disposables;
using Engine.Components;
using Engine.Components.Messaging;
using Engine.Core.Entities;
using Engine.Core.Serialization;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
{
	public class VisitorPosition : Component
	{
		public int CurrentTick { get; private set; }

		public int Position { get; private set; }

		public IEntity Host { get; private set; }

		private IDisposable _visitorSubscription;

		public VisitorPosition(IEntity entity) 
			: base(entity)
		{

			// host subscribe to messages from the visitor
			// visitor subscribe to messages from the host
		}

		public void SetHost(IEntity host)
		{
			Host = host;
			_visitorSubscription?.Dispose();
			_visitorSubscription = new CompositeDisposable
			{
				Host.Subscribe(Entity),
				Entity.Subscribe(Host),
			};
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

		protected override void OnDispose()
		{
			_visitorSubscription?.Dispose();
			base.OnDispose();
		}
	}
}
