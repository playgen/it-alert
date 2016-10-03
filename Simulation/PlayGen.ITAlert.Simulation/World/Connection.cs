using System;
using System.Linq;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Common.Serialization;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Interfaces;

namespace PlayGen.ITAlert.Simulation.World
{
	public class Connection : Node<ConnectionState>
	{
		/// <summary>
		/// The cost (bandwidth) of this connection
		/// </summary>
		[SyncState(StateLevel.Setup)]
		private int RelativeWeight { get; set; }

		/// <summary>
		/// The total cost of this connection (bandwidth * length)
		/// </summary>
		[SyncState(StateLevel.Setup)]
		public int Weight { get; private set; }

		#region connection properties

		[SyncState(StateLevel.Setup)]
		public Subsystem Head { get; private set; }
		[SyncState(StateLevel.Setup)]
		public Subsystem Tail { get; private set; }

		[SyncState(StateLevel.Setup)]
		public VertexDirection HeadPosition { get; private set; }
		[SyncState(StateLevel.Setup)]
		public VertexDirection TailPosition { get; private set; }

		#endregion

		#region constructors

		/// <summary>
		/// Create a new connection
		/// </summary>
		/// <param name="simulation"></param>
		/// <param name="head">Origin of connection</param>
		/// <param name="headDirection">Direction connection leaves origin</param>
		/// <param name="tail">Destination of connection</param>
		/// <param name="weight">Weight coefficient of connection [1-9]</param>
		public Connection(ISimulation simulation, Subsystem head, VertexDirection headDirection, Subsystem tail, int weight)
			: this(simulation, head, headDirection, tail, weight, SimulationConstants.Positions)
		{
			
		}

		/// <summary>
		/// Create a new connection
		/// </summary>
		/// <param name="simulation"></param>
		/// <param name="head">Origin of connection</param>
		/// <param name="headDirection">Direction connection leaves origin</param>
		/// <param name="tail">Destination of connection</param>
		/// <param name="weight">Weight coefficient of connection [1-9]</param>
		/// <param name="positions">Number of position steps for simulation</param>
		protected Connection(ISimulation simulation, Subsystem head, VertexDirection headDirection, Subsystem tail, int weight, int positions) 
			: base(simulation, EntityType.Connection, positions)
		{
			if (head == null)
			{
				throw new ArgumentNullException(nameof(head));
			}
			Head = head;

			if (tail == null)
			{
				throw new ArgumentNullException(nameof(tail));
			}
			Tail = tail;

			// set up graph connections
			HeadPosition = headDirection;
			TailPosition = headDirection.OppositePosition();

			head.AddExitPosition(HeadPosition, this);
			tail.AddEntrancePosition(TailPosition, this.Head);

			AddEntrancePosition(TailPosition, head);
			AddExitPosition(HeadPosition, tail);

			// calculate weight
			if (weight < SimulationConstants.ConnectionMinWeight || weight > SimulationConstants.ConnectionMaxWeight)
			{
				throw new ArgumentOutOfRangeException(nameof(weight), $"Value must be between {SimulationConstants.ConnectionMinWeight} and {SimulationConstants.ConnectionMaxWeight}, inclusive.");
			}
			var length = Math.Max(1, Math.Max(Math.Abs(Head.X - Tail.X), Math.Abs(Head.Y - Tail.Y)));
			RelativeWeight = weight;
			Weight = Positions*weight*length;

		}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected Connection()
		{

		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="visitor"></param>
		/// <param name="source">ignored for connections</param>
		/// <param name="offset"></param>
		public override void AddVisitor(IVisitor visitor, INode source, int offset)
		{
			// subsystems tick before connections:
			// if there is overflow movement (offset > 0) then we shouldnt move this again in the same tick, so set the tick counter on the position
			VisitorPositions.Add(visitor.Id, new VisitorPosition(visitor, offset, offset > 0 ? CurrentTick + 1 : CurrentTick));

		}
		
		#region state snapshot

		public override ConnectionState GenerateState()
		{
			// return values that only this class knows about, anything else will be in the other entity's state

			// all we care about are the visitors positions, so generate a dictionary keyed by the entity id
			var visitors = VisitorPositions.ToDictionary(k => k.Key, v => v.Value.Position);

			var state = new ConnectionState(Id)
			{
				Head = Head.Id,
				Tail = Tail.Id,
				RelativeWeight = RelativeWeight,
				Weight = Weight,
				VisitorPositions = visitors,
			};
			return state;
		}

		#endregion

		protected override void OnTick()
		{
			//base.OnTick();
			MoveVisitors();
		}

		private void MoveVisitors()
		{
			foreach (var visitorPosition in VisitorPositions.ToArray())
			{
				var actor = visitorPosition.Value.Visitor as IActor;
				if (actor != null)
				{
					var position = visitorPosition.Value.Position;
					var nextPosition = (position + actor.Speed);

					if (position == (Weight-1) || nextPosition >= Weight)
					{
						var overflow = Math.Max(nextPosition - Weight, 0);
						VisitorLeaving(actor, Tail, overflow);
						continue;
					}
					visitorPosition.Value.UpdatePosition(nextPosition, CurrentTick);
				}
			}
		}

	}
}
