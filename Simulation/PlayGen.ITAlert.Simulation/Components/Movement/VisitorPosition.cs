using System;
using System.ComponentModel;
using Engine.Entities;
using Newtonsoft.Json;
using IComponent = Engine.Components.IComponent;

//using Subsystem.Reactive.Disposables;

namespace PlayGen.ITAlert.Simulation.Components.Movement
{
	public class VisitorPosition : IComponent
	{
		public int CurrentTick { get; private set; }

		//[DefaultValue(-1f)]
		public decimal PositionDecimal { get; set; }

		[JsonIgnore]
		public int Position
		{
			get { return (int) PositionDecimal; }
			set { PositionDecimal = value; }
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

		/// <summary>
		/// Provide a tick safe method to update the current position
		/// </summary>
		/// <param name="position"></param>
		/// <param name="currentTick"></param>
		/// <returns>True if the position was updated. False if this position has been updated already this tick.</returns>
		public bool SetPosition(decimal position, int currentTick)
		{
			if (currentTick > CurrentTick)
			{
				PositionDecimal = position;
				CurrentTick = currentTick;
				return true;
			}
			return false;
		}
	}
}
