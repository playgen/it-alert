using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Interfaces;
using PlayGen.ITAlert.Simulation.Tests.Actors;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Tests.World
{
	[TestFixture]
	public class ConnectionTests
	{
		[Test]
		public void TestConstructor()
		{
			var sim = new TestSimulation();
			var head = new TestSubsystem(sim, 0, null, SimulationConstants.Positions);
			var tail = new TestSubsystem(sim, 1, null, SimulationConstants.Positions);
			var connection = new TestConnection(sim, head, VertexDirection.Top, tail, 1, SimulationConstants.Positions);
			
			Assert.AreEqual(connection.Head, head);
			Assert.AreEqual(connection.Tail, tail);

			Assert.IsTrue(head.ExitNodePositions.ContainsKey(connection.Id));
			Assert.IsFalse(head.EntranceNodePositions.ContainsKey(connection.Id));
			Assert.AreEqual(VertexDirection.Top, head.ExitNodePositions[connection.Id].Direction);

			Assert.IsTrue(tail.EntranceNodePositions.ContainsKey(head.Id));
			Assert.IsFalse(tail.ExitNodePositions.ContainsKey(connection.Id));
			Assert.AreEqual(VertexDirection.Bottom, tail.EntranceNodePositions[head.Id].Direction);
		}

		[Test]
		public void TestAddVisitor()
		{
			var sim = new TestSimulation();
			var actor = new TestPlayer(sim, 0);

			var head = new TestSubsystem(sim, 0, null, SimulationConstants.Positions);
			var tail = new TestSubsystem(sim, 1, null, SimulationConstants.Positions);
			var connection = new TestConnection(sim, head, VertexDirection.Top, tail, 1, SimulationConstants.Positions);

			connection.AddVisitor(actor, head, 0);

			Assert.True(connection.VisitorPositions.ContainsKey(actor.Id));
			Assert.AreEqual(0, connection.VisitorPositions[actor.Id].Position);
		}

		public struct GraphParams
		{
			public int Positions { get; set; }
			public VertexDirection Direction { get; set; }
			public int Weight { get; set; }

			public override string ToString()
			{
				return $"Positions: {Positions}, Direction: {Direction}, Weight: {Weight}";
			}
		}

		public struct VisitorMovementParams
		{
			public int Speed { get; set; }
			public int Ticks { get; set; }
			public int Overflow { get; set; }
			public int InitialPosition { get; set; }

			public override string ToString()
			{
				return $"Speed: {Speed}, Ticks: {Ticks}, Overflow: {Overflow}, InitialPosition: {InitialPosition}";
			}
		}

		#region single visitor

		public static TestCaseData[] SingleVisitorCases = new TestCaseData[]
		{
			new TestCaseData(
				new GraphParams() { Positions = 24, Direction = VertexDirection.Bottom, Weight = 1 },
				new VisitorMovementParams() { Speed = 1, Ticks = 24, Overflow = 0}
			).SetName("Speed 1, Bottom, Weight 1"),
			new TestCaseData(
				new GraphParams() { Positions = 24, Direction = VertexDirection.Right, Weight = 1 },
				new VisitorMovementParams() { Speed = 1, Ticks = 24, Overflow = 0}
			).SetName("Speed 1, Right, Weight 1"),
			new TestCaseData(

			// speed multipliers
				new GraphParams() { Positions = 24, Direction = VertexDirection.Bottom, Weight = 1 },
				new VisitorMovementParams() { Speed = 2, Ticks = 12, Overflow = 0 }
			).SetName("Speed 2, Bottom, Weight 1"),
			new TestCaseData(
				new GraphParams() { Positions = 24, Direction = VertexDirection.Bottom, Weight = 1 },
				new VisitorMovementParams() { Speed = 3, Ticks = 8, Overflow = 0}
			).SetName("Speed 3, Bottom, Weight 1"),
			new TestCaseData(
				new GraphParams() { Positions = 24, Direction = VertexDirection.Bottom, Weight = 1 },
				new VisitorMovementParams() { Speed = 4, Ticks = 6, Overflow = 0 }
			).SetName("Speed 4, Bottom, Weight 1"),
			new TestCaseData(

			// overflow
				new GraphParams() { Positions = 24, Direction = VertexDirection.Bottom, Weight = 1 },
				new VisitorMovementParams() { Speed = 5, Ticks = 5, Overflow = 1 }
			).SetName("Speed 5, Bottom, Weight 1, Overflow"),
			new TestCaseData(
				new GraphParams() { Positions = 24, Direction = VertexDirection.Bottom, Weight = 1 },
				new VisitorMovementParams() { Speed = 7, Ticks = 4, Overflow = 4}
			).SetName("Speed 7, Bottom, Weight 1, Overflow"),
			new TestCaseData(

			// weight
				new GraphParams() { Positions = 24, Direction = VertexDirection.Bottom, Weight = 2 },
				new VisitorMovementParams() { Speed = 1, Ticks = 48, Overflow = 0}
			).SetName("Speed 1, Bottom, Weight 2"),
			new TestCaseData(
				new GraphParams() { Positions = 24, Direction = VertexDirection.Right, Weight = 3 },
				new VisitorMovementParams() { Speed = 1, Ticks = 72, Overflow = 0}
			).SetName("Speed 1, Right, Weight 3"),

			//initial offset
			new TestCaseData(
				new GraphParams() { Positions = 24, Direction = VertexDirection.Bottom, Weight = 1 },
				new VisitorMovementParams() { Speed = 1, Ticks = 19, Overflow = 0, InitialPosition = 6}
			).SetName("Speed 1, Bottom, Weight 1, Offset 6"),
			new TestCaseData(
				new GraphParams() { Positions = 24, Direction = VertexDirection.Right, Weight = 1 },
				new VisitorMovementParams() { Speed = 2, Ticks = 10, Overflow = 0, InitialPosition = 6}
			).SetName("Speed 2, Right, Weight 1, Offset 6"),

			// combination
			new TestCaseData(
				new GraphParams() { Positions = 24, Direction = VertexDirection.Bottom, Weight = 1 },
				new VisitorMovementParams() { Speed = 2, Ticks = 11, Overflow = 1, InitialPosition = 5}
			).SetName("Speed 2, Bottom, Weight 1, Offset 5"),
			new TestCaseData(
				new GraphParams() { Positions = 24, Direction = VertexDirection.Right, Weight = 2 },
				// start at position 5, have 43 steps to traverse at 2 per tick = 22, plus one for offset
				new VisitorMovementParams() { Speed = 2, Ticks = 23, Overflow = 1, InitialPosition = 5}
			).SetName("Speed 2, Right, Weight 2, Offset 5"),

		};

		[TestCaseSource(nameof(SingleVisitorCases))]
		public void TestSingleVisitorMovement(GraphParams graphParams, VisitorMovementParams visitorParams)
		{
			var sim = new TestSimulation();
			var head = new TestSubsystem(sim, 0, null, graphParams.Positions);
			var tail = new TestSubsystem(sim, 1, null, graphParams.Positions);

			var connection = new TestConnection(sim, head, graphParams.Direction, tail, graphParams.Weight, graphParams.Positions);

			var actor = new TestPlayer(sim, visitorParams.Speed);

			connection.AddVisitor(actor, head, visitorParams.InitialPosition);

			var tick = 0;
			tail.VisitorEntered += (sender, args) =>
			{
				Assert.NotNull(args.Visitor);
				Assert.AreEqual(visitorParams.Ticks, tick);
			};

			for (var i = 0; i < visitorParams.Ticks; i++)
			{
				connection.Tick(++tick);
			}


			Assert.True(tail.VisitorPositions.ContainsKey(actor.Id));
			var position = graphParams.Direction.OppositePosition().ToPosition(graphParams.Positions) + visitorParams.Overflow;
			Assert.AreEqual(position, tail.VisitorPositions[actor.Id].Position);
		}

		#endregion

		#region single visitor

		public static object[] MultiVisitorCases = new object[]
		{
			new object[]
			{
				new GraphParams() { Positions = 24, Direction = VertexDirection.Bottom, Weight = 1 },
				new VisitorMovementParams[]
				{
					new VisitorMovementParams() { Speed = 1, Ticks = 24, Overflow = 0},
					new VisitorMovementParams() { Speed = 1, Ticks = 17, Overflow = 0, InitialPosition = 8},	// entering with initial position > 0 implies overflow from the previous movement, so +1 tick to traverse
				}
			},
			new object[]
			{
				new GraphParams() {Positions = 24, Direction = VertexDirection.Bottom, Weight = 1},
				new VisitorMovementParams[]
				{
					new VisitorMovementParams() { Speed = 1, Ticks = 24, Overflow = 0},
					new VisitorMovementParams() { Speed = 2, Ticks = 12, Overflow = 0},
				}
			},
			new object[]
			{
				new GraphParams() {Positions = 24, Direction = VertexDirection.Bottom, Weight = 1},
				new VisitorMovementParams[]
				{
					new VisitorMovementParams() { Speed = 1, Ticks = 24, Overflow = 0},
					new VisitorMovementParams() { Speed = 2, Ticks = 11, Overflow = 1, InitialPosition = 5},
				}
			},
		};

		[TestCaseSource(nameof(MultiVisitorCases))]
		public void TestMultipleVisitorMovement(GraphParams graphParams, VisitorMovementParams[] visitorParams)
		{
			var sim = new TestSimulation();
			var head = new TestSubsystem(sim, 0, null, graphParams.Positions);
			var tail = new TestSubsystem(sim, 1, null, graphParams.Positions);

			var connection = new TestConnection(sim, head, graphParams.Direction, tail, graphParams.Weight, graphParams.Positions);

			var actors = visitorParams.ToDictionary(k => (IVisitor)new TestPlayer(sim, k.Speed), v => v);

			foreach(var testActor in actors)
			{
				connection.AddVisitor(testActor.Key, head, testActor.Value.InitialPosition);
			}
			
			var tick = 0;

			tail.VisitorEntered += (sender, args) =>
			{
				Assert.NotNull(args.Visitor);
				Assert.True(actors.ContainsKey(args.Visitor));
				var visitorParam = actors[args.Visitor];
				Assert.AreEqual(visitorParam.Ticks, tick);
			};

			for (var i = 0; i < visitorParams.Max(vp => vp.Ticks); i++)
			{
				connection.Tick(++tick);
			}

			foreach (var testActor in actors)
			{
				Assert.True(tail.VisitorPositions.ContainsKey(testActor.Key.Id));
				var position = graphParams.Direction.OppositePosition().ToPosition(graphParams.Positions) + testActor.Value.Overflow;
				Assert.AreEqual(position, tail.VisitorPositions[testActor.Key.Id].Position);
				
			}

		}

		#endregion

	}
}

