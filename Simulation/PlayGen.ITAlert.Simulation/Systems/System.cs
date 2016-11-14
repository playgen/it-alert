using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Core.Entities;
using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Systems.Behaviours;
using PlayGen.ITAlert.Simulation.Systems.Extensions;
using PlayGen.ITAlert.Simulation.Visitors;
using PlayGen.ITAlert.Simulation.Visitors.Actors;
using PlayGen.ITAlert.Simulation.Visitors.Actors.Intents;
using PlayGen.ITAlert.Simulation.Visitors.Actors.Npc;

namespace PlayGen.ITAlert.Simulation.Systems
{
	public class System : IEntity, INode
	{
		#region items

		//public bool HasActiveItem => Items.Any(i => i != null && i.IsActive);

		#endregion

		#region constructors

		public System(ISimulation simulation) 
			: this(simulation)
		{
		}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected System()
		{

		}

		#endregion

		#region state snapshot

		//public override SystemState GenerateState()
		//{
		//	// return values that only this class knows about, anything else will be in the other entity's state
		//	var playerPositions = VisitorPositionState
		//		.Where(v => v.Value.Visitor is Player)
		//		.ToDictionary(k => k.Key, v => v.Value.Position);

		//	var infection = VisitorPositionState.Values.SingleOrDefault(v => v.Visitor is IInfection)?.Visitor as IInfection;

		//	var state = new SystemState(Id)
		//	{
		//		LogicalId = LogicalId,
		//		// static
		//		X = X,
		//		Y = Y,
		//		Name = Name,
		//		// ui state
		//		//TODO: reimplement
		//		//Health = (float)Health /SimulationConstants.MaxHealth,
		//		//Shield = (float)Shield /SimulationConstants.MaxShield,
		//		//Disabled = IsDead,
		//		//Infection = infection?.Visible ?? false ? infection.Id : (int?)null,
		//		Infection = infection?.Id,
		//		VisitorPositions = playerPositions,
		//		ItemPositions = Items.Select(i => i?.Id).ToArray(),
		//		HasActiveItem = HasActiveItem,	
		//	};
		//	return state;
		//}

		public override void OnDeserialized()
		{
			base.OnDeserialized();

		}


		#endregion

	}
}
